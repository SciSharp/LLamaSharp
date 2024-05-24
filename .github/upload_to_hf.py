from huggingface_hub import HfApi
import argparse

def parse_args():
    parser = argparse.ArgumentParser()
    parser.add_argument("--token", type=str, default=None)
    parser.add_argument("--repo-id", type=str, default="AsakusaRinne/LLamaSharpNative")
    parser.add_argument("--folder", type=str, required=True)
    parser.add_argument("--revision", type=str, required=True)
    parser.add_argument("--commit_message", type=str, default=None)
    return parser.parse_args()

if __name__ == '__main__':
    args = parse_args()
    api = HfApi(token=args.token)
    
    if args.commit_message is None:
        args.commit_message = f"Upload native library of version {args.revision}"
    
    print(f'Creating branch {args.revision} ...')
    try:
        api.delete_branch(repo_id=args.repo_id, branch=args.revision, token=args.token)
        print(f'Branch {args.revision} already exists, deleting it ...')
    except:
        pass
    api.create_branch(repo_id=args.repo_id, branch=args.revision, token=args.token, exist_ok=False)
    # upload the folder to huggingface repo
    print(f'Uploading folder to huggingface repo with revision {args.revision} ...')
    api.upload_folder(
        repo_id=args.repo_id, 
        folder_path=args.folder,
        path_in_repo="runtimes", 
        token=args.token, 
        revision=args.revision, 
    )
    
    
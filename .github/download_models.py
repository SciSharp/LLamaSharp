from huggingface_hub import hf_hub_download
import argparse

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--model-list', type=str, required=True)
    parser.add_argument('--model-dir', type=str, required=True)
    parser.add_argument('--endpoint', type=str, default='https://huggingface.co')
    args = parser.parse_args()
    
    with open(args.model_list, 'r') as f:
        repo_id, filename = f.readline().split(',')
    
    hf_hub_download(
        repo_id=repo_id, 
        filename=filename, 
        local_dir=args.model_dir, 
        local_dir_use_symlinks=False, 
        endpoint=args.endpoint
    )
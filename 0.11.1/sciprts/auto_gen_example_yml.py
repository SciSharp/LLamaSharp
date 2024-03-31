import os

dir = 'Examples'

if __name__ == '__main__':
    res = []
    
    # loop all the files of `dir`
    for root, dirs, files in os.walk(dir):
        for file in files:
            with open(os.path.join(root, file), 'r', encoding='utf-8') as f:
                first_line = f.readline()
            title = first_line.split('#')[-1]
            filename = file.split('/')[-1].split('\\')[-1]
            res.append(f'- {title.strip()}: {dir}/{filename}')
    
    for item in res:
        print(item)
            
            
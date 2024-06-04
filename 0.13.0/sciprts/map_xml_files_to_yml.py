import os

def generate_string_list(folder_path, prefix):
    file_names = os.listdir(folder_path) 
    string_list = []
    for file_name in file_names:
        new_string = f"- {'.'.join(file_name.split('.')[:-1])}: {prefix}{file_name}"
        string_list.append(new_string)
    return string_list

folder_path = "./docs/xmldocs" 
prefix = "./xmldocs/" 

string_list = generate_string_list(folder_path, prefix)
result = '\n'.join(string_list)
print(result)

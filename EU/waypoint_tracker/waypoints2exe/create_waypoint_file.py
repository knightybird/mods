import os

filename = "waypoint.py"
base_name, ext = os.path.splitext(filename)
exe_list_index = 3  # The line number where the exe list starts in setup.py

i = 1
for n in range(20):
    while os.path.exists(f"{base_name}-{i}{ext}"):
        i += 1

    new_filename = f"{base_name}-{i}{ext}"

    with open(filename, 'r') as f:
        content = f.read()

    content = content.replace("i = 0", f"i = {i-1}")

    with open(new_filename, 'w') as f:
        f.write(content)

    print(f"Created duplicate file: {new_filename}")

    # Update setup.py
    # Append the new file to the exe list in setup.py
    with open("setup.py", "r") as setup_file:
        lines = setup_file.readlines()

    exe_line = f"    Executable(\"{new_filename}\", base=\"Win32GUI\"),\n"
    lines.insert(exe_list_index, exe_line)

    with open("setup.py", "w") as setup_file:
        setup_file.writelines(lines)
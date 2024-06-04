import os

filename = "waypoint.py"
base_name, ext = os.path.splitext(filename)

i = 0
while os.path.exists(f"{base_name}-{i}{ext}"):
    i += 1

new_filename = f"{base_name}-{i}{ext}"

with open(filename, 'r') as f:
    content = f.read()

content = content.replace("i = 0", f"i = {i}")

with open(new_filename, 'w') as f:
    f.write(content)

print(f"Created duplicate file: {new_filename}")
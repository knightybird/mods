## Git instructions
mkdir eu

cd eu

git init

git remote add -f origin https://github.com/knightybird/mods.git

git config core.sparseCheckout true
git sparse-checkout init
git sparse-checkout add EU/lootius
git sparse-checkout set EU/lootius

git pull origin main

## install instructions
Run Termux (linux on android)

pkg update

pkg install tur-repo

pkg install python3.9

pip3.9 install requirements.txt

## create .env file
nano .env

`USER="your_username"` (note: USERNAME seems to use literal PC user instead of .env; do not use word USERNAME for .env)


`PASS="your_password"`


Press Ctrl+O to save

Press Ctrl+X to exit

Press Y and then Enter to confirm saving changes and exit

`ls -a ` (confirm if env file is present)


## run
python3.9 main.py
import pyperclip


def main():
    i = 0
    with open('waypoints.txt', 'r') as file:
        lines = file.readlines()
        pyperclip.copy(lines[i].strip())


if __name__ == '__main__':
    main()

import os
import time

from dotenv import load_dotenv

load_dotenv(dotenv_path=".env")

VM_chat_log_file = os.getenv("VM_FILE_PATH")
VM_shared_folder = os.getenv("VM_SHARED_FOLDER")  # output path
PC_shared_folder = os.getenv("PC_SHARED_FOLDER")


def create_directory_if_not_exists(directory_path):
    if not os.path.exists(directory_path):
        os.makedirs(directory_path)


def print_line_count(file_path):
    with open(file_path, 'r', encoding='utf-8', ) as file:
        lines = file.readlines()
    print(f"Number of lines in chat log file: {len(lines)}")


def get_log_files(folder):
    return sorted(os.listdir(folder))


def find_chat_log_file():
    '''
    Finds file in folders, creates folders if not found.
    :return: chat.log file
    '''
    if os.path.exists(VM_shared_folder):
        if os.path.exists(VM_chat_log_file):  # If on VM then copy into vm shared
            create_directory_if_not_exists(os.path.join(VM_shared_folder, "chat_logs"))
        return VM_chat_log_file

    elif os.path.exists(PC_shared_folder):
        pc_shared_sub_folder = os.path.join(PC_shared_folder, "chat_logs")
        create_directory_if_not_exists(pc_shared_sub_folder)
        pc_shared_file = os.path.join(pc_shared_sub_folder, "chat.log")

        if not os.path.exists(pc_shared_file):
            with open(pc_shared_file, 'w') as f:
                pass  # create an empty file
            print(f"Created a new chat.log file at {os.path.abspath(pc_shared_file)}")
        else:
            print(f"chat.log file already exists at {os.path.abspath(pc_shared_file)}")

        return pc_shared_file
    else:
        raise FileNotFoundError("Chat log file not found")


def monitor_chat_log_to_channels(chat_log_file, output_path):
    # Create output directory if it doesn't exist
    if not os.path.exists(output_path):
        os.makedirs(output_path)

    channel_files = {}  # dictionary to store file handles for each channel

    with open(chat_log_file, 'r', encoding='utf-8') as file:
        lines = file.readlines()

    last_line_seen = len(lines)

    while True:
        time.sleep(5)  # Check every 5 seconds

        with open(chat_log_file, 'r', encoding='utf-8') as file:
            current_lines = file.readlines()

        new_lines = current_lines[last_line_seen:]
        if new_lines:
            last_line_seen = len(current_lines)

            for line in new_lines:
                # Extract channel name from the line
                channel_name = line.split('[')[1].split(']')[0]

                # Open the channel file if it doesn't exist
                if channel_name not in channel_files:
                    channel_files[channel_name] = open(os.path.join(output_path, f"{channel_name.lower()}.log"), 'a',
                                                       encoding='utf-8')

                # Write the line to the corresponding channel file
                channel_files[channel_name].write(line)
                channel_files[channel_name].flush()  # Force the file to be written to disk

                # # Print the line to the console (optional)
                # print(line.strip())


def tail_chat(chat_log_file):
    with open(chat_log_file, 'r', encoding='utf-8') as file:
        lines = file.readlines()

    last_line_seen = len(lines)

    while True:
        time.sleep(5)  # Check every 5 seconds

        with open(chat_log_file, 'r', encoding='utf-8') as file:
            current_lines = file.readlines()

        new_lines = current_lines[last_line_seen:]
        if new_lines:
            last_line_seen = len(current_lines)
            print("\n".join(new_lines))


def main():
    chat_log_file = find_chat_log_file()
    print_line_count(chat_log_file)

    if os.path.exists(VM_chat_log_file):  # If on VM then copy into vm shared
        vm_shared_sub_folder = os.path.join(VM_shared_folder, "chat_logs")
        output_path = "channel_logs"
        monitor_chat_log_to_channels(chat_log_file, output_path)
        # tail_chat(chat_log_file)
        with open(chat_log_file, 'r', encoding='utf-8') as input_file:
            with open(VM_shared_folder, 'w') as output_file:
                output_file.write("text.txt")

    elif os.path.exists(PC_shared_folder):  # If on PC then run chatlogger

        folder = "channel_logs"
        log_files = get_log_files(folder)

        print("Select a log file:")
        for i, log_file in enumerate(log_files):
            print(f"{i}. {log_file}")

        choice = input("Enter the number of your chosen log file: ")

        try:
            choice = int(choice)
            if choice < 0 or choice >= len(log_files):
                print("Invalid choice. Please try again.")
                return
            chosen_log_file = log_files[choice]
            # print(f"You have chosen: {chosen_log_file}")
            print(f"watching channel: {chosen_log_file}")
            chat_log = os.path.join(folder, chosen_log_file)
            tail_chat(chat_log)

        except ValueError:
            print("Invalid input. Please enter a number.")


if __name__ == '__main__':
    main()

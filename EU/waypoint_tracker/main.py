import time

import psutil
import pyautogui

pyautogui.FAILSAFE = False
import pydirectinput

pydirectinput.FAILSAFE = False
import pyperclip
import win32api
import win32gui  # pip install pywin32
import win32process
import win32con
import time
import mouse
import keyboard
from pynput.keyboard import Key, Controller

from pywinauto import Application


def get_process_id(name):
    """
    Returns the process ID of the process with the specified name.

    :param name: The name of the process.
    :return: The process ID of the process.
    """
    for proc in psutil.process_iter():
        if proc.name() == name:
            return proc.pid

    return None


def set_window_focus(pid):
    """
    Brings the window with the specified process ID to the foreground.

    :param pid: The process ID of the window.
    """

    def enum_windows_callback(hwnd, lparam):
        """
        Callback function for EnumWindows.

        :param hwnd: The handle of the window.
        :param lparam: The parameter passed to EnumWindows.
        """
        if win32process.GetWindowThreadProcessId(
                hwnd) == win32process.GetCurrentProcessId() and win32gui.IsWindowVisible(hwnd):
            win32gui.SetForegroundWindow(hwnd)
            return False
        return True

    process_id = (pid, 0)
    win32gui.EnumWindows(enum_windows_callback, None)


def revive():
    # If image is not found After 3 seconds wait time...
    time.sleep(1)
    t_end = time.time() + 3
    while time.time() < t_end:
        try:
            x, y = pyautogui.locateCenterOnScreen("images/revive-button.JPG", confidence=0.8, grayscale=True)
            time.sleep(1)
            pydirectinput.mouseDown(x, y)
            pydirectinput.mouseUp(x, y)
            break

        except:
            pass


def face_to_waypoint(x, y, new_x, new_y, steps=10):
    x_diff = new_x - x
    y_diff = new_y - y
    print(x, y)
    print(new_x, new_y)
    print("diffs", x_diff, y_diff)
    print("target position: ", new_x, new_y)

    mouse.move(x, y, absolute=True, duration=1)
    keyboard = Controller()
    keyboard.press(Key.alt_l)
    keyboard.release(Key.alt_l)
    time.sleep(5)

    current_x, current_y = pydirectinput.position()
    print("current pos", x, y)
    print("current pydirection pos", current_x, current_y)

    # Calculate the step size for each movement
    step_x = x_diff / steps  # You can adjust the number of steps
    step_y = y_diff / steps

    # Perform a series of small mouse movements to reach the target location
    for _ in range(steps):
        current_x += step_x
        current_y += step_y
        pydirectinput.moveTo(int(current_x / 7), int(current_y / 7))
        time.sleep(0.5)

    print("new position: ", pydirectinput.position())
    keyboard.press(Key.alt_l)
    keyboard.release(Key.alt_l)
    print("new position: ", pyautogui.position())


def track_waypoint():
    button_location = None
    while True:
        try:
            button_location = pyautogui.locateCenterOnScreen("images/waypoint.JPG", confidence=0.7, grayscale=True)
            print('Found it!')
            print(pyautogui.size())
            print(button_location[0], button_location[1])
            time.sleep(1)
            break
        except Exception as e:
            print(f"An error occurred: {e}")
            pass
    if button_location is not None:
        face_to_waypoint(330, 240, button_location[0], button_location[1])
    else:
        print("Button not found.")


def move_to_waypoint():
    keyboard = Controller()
    keyboard.press(Key.alt_l)
    keyboard.release(Key.alt_l)


    keyboard.press('w')
    while pyautogui.locateCenterOnScreen("images/waypoint.JPG", confidence=0.5, grayscale=True) is not None:
        time.sleep(0.5)

    keyboard.release('w')

    keyboard.press(Key.alt_l)
    keyboard.release(Key.alt_l)


def get_waypoint(waypoint):
    print(pyautogui.size())
    with open("notepad.txt") as f:
        text = f.read()

    # Copy the text to the clipboard
    print(text)
    pyperclip.copy(text)

    # Move the mouse to the location of the text field
    pyautogui.moveTo(1900, 900)

    # Simulate a left-click to select the text field
    pyautogui.click()
    pyautogui.PAUSE = 1.0
    pyautogui.hotkey('enter')
    time.sleep(1)
    pyautogui.typewrite(pyperclip.paste())
    pyautogui.PAUSE = 1.0
    pyautogui.hotkey('enter')


def move():
    pydirectinput.mouseDown(button='right')
    time.sleep(1)
    pydirectinput.mouseUp(button='right')


def main():
    pid = get_process_id("Entropia.exe")
    set_window_focus(pid)

    if pid is not None:
        app = Application().connect(process=pid)
        app.top_window().set_focus()
    # revive()
    time.sleep(2)
    track_waypoint()
    move_to_waypoint()


if __name__ == '__main__':
    main()

# See PyCharm help at https://www.jetbrains.com/help/pycharm/

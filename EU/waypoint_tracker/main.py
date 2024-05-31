import time

import psutil
import pyautogui
import pydirectinput as pydirectinput
import pyperclip
import win32api
import win32gui  # pip install pywin32
import win32process
import win32con
import time

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


def track_waypoint():
    while True:
        try:
            button_location = pyautogui.locateCenterOnScreen("images/waypoint.JPG", confidence=0.7, grayscale=True)
            print('Found it!')
            print(pyautogui.size())
            print(pyautogui.position())
            print(button_location[0], button_location[1])
            time.sleep(1)
            pydirectinput.moveTo(520, 380)

            # pydirectinput.mouseDown(x=520, y=380, button='right')
            pydirectinput.moveTo(button_location[0], button_location[1], 5)
            # pydirectinput.mouseUp(x=520, y=380, button='right')

            # pyautogui.mouseDown(x=1413, y=676, button='right')
            # pyautogui.mouseUp(x=1413, y=676, button='right')
            # pyautogui.mouseDown(x=1413, y=676, button='left')
            # time.sleep(1)
            # pyautogui.mouseUp(x=1413, y=676, button='left')
            break
        except:
            pass


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
    time.sleep(5)
    # track_waypoint()
    # Move the mouse to the specified position
    win32api.SetCursorPos((520, 380))

    # Click the left mouse button at the current mouse position
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)

    # Release the left mouse button
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)

    # Drag the mouse to the new position
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
    win32api.SetCursorPos((600, 380))
    win32api.mouse_event(win32con.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
    # move()


# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    main()

# See PyCharm help at https://www.jetbrains.com/help/pycharm/

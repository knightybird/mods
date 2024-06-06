import psutil
import pyautogui

pyautogui.FAILSAFE = False
import pydirectinput

pydirectinput.FAILSAFE = False
import win32gui  # pip install pywin32
import win32process
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


def get_pid():
    pid = get_process_id("Entropia.exe")
    if pid is not None:
        app = Application().connect(process=pid)
        app.top_window().set_focus()


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


def face_to_waypoint(new_x, new_y, steps=12):
    current_x, current_y = pyautogui.position()
    x_diff = new_x - current_x
    y_diff = new_y - current_y
    step_x = x_diff / steps  # You can adjust the number of steps
    step_y = y_diff / steps

    # Perform a series of small mouse movements to reach the target location
    for _ in range(steps):
        pydirectinput.moveRel(xOffset=int(step_x * 12), yOffset=int(step_y * 12), relative=True,
                              disable_mouse_acceleration=True)
        print("pydirection pos", pyautogui.position())
        time.sleep(0.1)


def move_to_tracked_waypoint(waypoint_image, crosshair_x, crosshair_y, delay=5):
    wait_time = delay
    search_time = 0.1
    elapsed_time = 0
    start_time = time.time()
    last_found_time = time.time()  # current time
    is_moving = False
    active_state = True
    is_alt = False

    keyboard = Controller()

    while True:
        try:
            time.sleep(search_time)
            button_location = pyautogui.locateCenterOnScreen(waypoint_image, confidence=0.5, grayscale=True)

            if button_location is not None:
                elapsed_time = time.time() - start_time

                if not is_moving:
                    # mouse.move(crosshair_x, crosshair_y, absolute=True)
                    keyboard.press(Key.alt_l)
                    keyboard.release(Key.alt_l)
                    is_alt = True
                    time.sleep(3)

                    keyboard.press('w')
                    is_moving = True
                    time.sleep(0.5)

                if elapsed_time >= 3:
                    mouse.move(crosshair_x, crosshair_y, absolute=True)

                    start_time = time.time()
                    elapsed_time = 0
                    face_to_waypoint(button_location[0], button_location[1])
                    time.sleep(0.5)  # Wait x seconds before moving

                last_found_time = time.time()  # Resets the last found time

            else:
                elapsed_time = 0
                if is_moving:
                    is_moving = False
                    keyboard.release('w')
                    time.sleep(0.5)
                    if is_alt:
                        keyboard.press(Key.alt_l)
                        keyboard.release(Key.alt_l)
                        is_alt = False
                        time.sleep(1)

                else:
                    # If image not found and button released, then
                    # Switch to inactive state if image not found since x seconds
                    if time.time() - last_found_time >= wait_time:
                        active_state = False

                    continue

        except pyautogui.ImageNotFoundException:

            if is_moving:
                keyboard.release('w')  # Release the 'w' key immediately
                time.sleep(0.5)
                is_moving = False
                if is_alt:
                    keyboard.press(Key.alt_l)
                    keyboard.release(Key.alt_l)
                    is_alt = False
                    time.sleep(1)

            if active_state:
                continue
            else:
                # Stay in inactive state until the image is found
                keyboard.release('w')
                search_time = wait_time
                continue


def main():
    print(pyautogui.size())
    set_window_focus(get_pid())
    waypoint_image = "images/waypoint.jpg"
    move_to_tracked_waypoint(waypoint_image, 324, 250)


if __name__ == '__main__':
    main()

# See PyCharm help at https://www.jetbrains.com/help/pycharm/

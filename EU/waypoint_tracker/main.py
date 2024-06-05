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


def face_to_waypoint(x, y, new_x, new_y, steps=12):
    print("crosshair pos", x, y)
    print("target position: ", new_x, new_y)

    # current_x, current_y = pydirectinput.position()
    # crosshair_x = x
    # crosshair_y = y
    # # Calculate the difference between the current and target positions
    # dx, dy = crosshair_x - current_x, crosshair_y - current_y
    # for _ in range(steps):
    #     pydirectinput.move(int(dx / steps), int(dy / steps))
    #     print("pyautogui pos", pyautogui.position())
    #
    #     time.sleep(0.01)

    mouse.move(x, y, absolute=True)

    current_x, current_y = pyautogui.position()
    print("current pydirection pos", current_x, current_y)

    x_diff = new_x - current_x
    y_diff = new_y - current_y
    print("dx, dy: ", x_diff, y_diff)

    step_x = x_diff / steps  # You can adjust the number of steps
    step_y = y_diff / steps
    print("step x, y: ", int(step_x), int(step_y))
    # Perform a series of small mouse movements to reach the target location

    keyboard = Controller()
    keyboard.press(Key.alt_l)
    keyboard.release(Key.alt_l)
    time.sleep(3)
    # Perform a series of small mouse movements to reach the target location
    for _ in range(steps):
        # current_x += step_x
        # current_y += step_y
        pydirectinput.moveRel(xOffset=int(step_x * 12), yOffset=int(step_y * 12), relative=True,
                              disable_mouse_acceleration=True)
        # pydirectinput.moveTo(int(current_x / 7), int(current_y / 7))
        print("pydirection pos", pyautogui.position())
        time.sleep(0.1)

    # print("new position: ", pydirectinput.position())
    # print("new position: ", mouse.get_position())
    keyboard.press(Key.alt_l)
    keyboard.release(Key.alt_l)
    time.sleep(1)


def move_to_tracked_waypoint(waypoint_image, delay=5):
    wait_time = delay
    search_time = 0.1
    last_found_time = time.time()  # current time
    is_moving = False
    active_state = True

    while True:
        try:
            button_location = pyautogui.locateCenterOnScreen(waypoint_image, confidence=0.5, grayscale=True)
            time.sleep(search_time)

            if button_location is not None:

                if not is_moving:
                    face_to_waypoint(324, 250, button_location[0], button_location[1])

                    time.sleep(1)  # Wait x seconds before moving
                    is_moving = True
                    keyboard.press('w')
                last_found_time = time.time()  # Updates the last found time

            else:
                if is_moving:
                    is_moving = False
                    keyboard.release('w')

                else:
                    # If image not found and button released, then
                    # Switch to inactive state if image not found since x seconds
                    if time.time() - last_found_time >= wait_time:
                        active_state = False

                    continue

        except pyautogui.ImageNotFoundException:

            if is_moving:
                keyboard.release('w')  # Release the 'w' key immediately
                is_moving = False

            if active_state:
                continue

            else:
                # Stay in inactive state until the image is found
                keyboard.release('w')
                search_time = wait_time
                continue


def main():
    set_window_focus(get_pid())
    waypoint_image = "images/waypoint.jpg"
    move_to_tracked_waypoint(waypoint_image)


if __name__ == '__main__':
    main()

# See PyCharm help at https://www.jetbrains.com/help/pycharm/

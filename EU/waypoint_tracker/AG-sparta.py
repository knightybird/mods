from main import *


def main():
    set_window_focus(get_pid())
    waypoint_image = "images/waypoint.jpg"
    move_to_tracked_waypoint(waypoint_image)


if __name__ == '__main__':
    main()

# See PyCharm help at https://www.jetbrains.com/help/pycharm/

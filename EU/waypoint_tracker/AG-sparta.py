from main import *


def main():
    # set_window_focus(get_pid())

    waypoints = "wp_sparta.txt"


    print(get_waypoint(waypoints, 2))
    waypoint_image = "images/waypoint.jpg"
    move_to_tracked_waypoint(waypoint_image, waypoints)


if __name__ == '__main__':
    main()

# See PyCharm help at https://www.jetbrains.com/help/pycharm/

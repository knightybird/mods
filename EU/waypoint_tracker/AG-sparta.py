from main import *


def main():
    set_window_focus(get_pid())

    waypoints = "wp_sparta.txt"

    get_waypoint(waypoints)

    # paste_waypoint()

    toggle_hud()
    waypoint_image = "images/waypoint.jpg"
    track_waypoint(waypoint_image)
    move_to_waypoint(waypoint_image)
    toggle_hud()


if __name__ == '__main__':
    main()

# See PyCharm help at https://www.jetbrains.com/help/pycharm/

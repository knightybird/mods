# Waypoint Tracker

Welcome to the Waypoint Tracker project! This is a simple application that helps you track and manage waypoints in your games.

## Features

- Build with PyInstaller and cx_Freeze for cross-platform compatibility
- Easy-to-use command line interface
- Automatic waypoint file creation and setup.py updates with `create_waypoint_file.py` script

## Build Instructions

### PyInstaller

To build the application with PyInstaller, run the following commands:
```bash
pyinstaller -F AG-sparta.py
pyinstaller -F main.py
These commands will create standalone executable files for AG-sparta.py and main.py.

cx_Freeze
To build the application with cx_Freeze, cd to the waypoint_tracker/waypoints2exe folder and run the following command:

python setup.py bdist_msi
This will create a Windows installer for the Waypoint Tracker application.
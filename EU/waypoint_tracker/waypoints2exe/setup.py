from cx_Freeze import setup, Executable

exe = [
    Executable("waypoint-1.py", base="Win32GUI"),
    Executable("waypoint-2.py", base="Win32GUI"),
    Executable("waypoint-3.py", base="Win32GUI"),
    Executable("waypoint-4.py", base="Win32GUI"),
    Executable("waypoint-5.py", base="Win32GUI"),
    Executable("waypoint-6.py", base="Win32GUI"),
    Executable("waypoint-7.py", base="Win32GUI"),
    Executable("waypoint-8.py", base="Win32GUI"),
    Executable("waypoint-9.py", base="Win32GUI"),
    Executable("waypoint-10.py", base="Win32GUI"),
    Executable("waypoint-11.py", base="Win32GUI"),
    Executable("waypoint-12.py", base="Win32GUI"),
    Executable("waypoint-13.py", base="Win32GUI"),
    Executable("waypoint-14.py", base="Win32GUI"),
    Executable("waypoint-15.py", base="Win32GUI"),
    Executable("waypoint-16.py", base="Win32GUI"),
    Executable("waypoint-17.py", base="Win32GUI"),
    Executable("waypoint-18.py", base="Win32GUI"),
    Executable("waypoint-19.py", base="Win32GUI"),
    Executable("waypoint-20.py", base="Win32GUI"),


]


setup(
    name="waypoint-x",
    version="1.0",
    options={"build_exe": {"packages": ["pyperclip"],
                           "include_files": ["waypoints.txt"]}},
    executables=exe
)
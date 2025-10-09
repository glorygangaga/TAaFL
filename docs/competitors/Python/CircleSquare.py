import math

try:
    r = float(input("Enter the circle radius: "))
    if r < 0:
        print("Error: the radius cannot be negative.")
    else:
        area = math.pi * r * r
        print(f"The area of the circle is: {area:.6f}")
except ValueError:
    print("Error: please enter a numeric value for the radius.")

#include <iostream>
#include <iomanip>
#include <limits>

#define _USE_MATH_DEFINES
#include <math.h>

double readDouble(const std::string &prompt)
{
    double value;
    while (true)
    {
        std::cout << prompt;
        if (std::cin >> value)
        {
            if (value >= 0)
            {
                return value;
            }
            else
            {
                std::cout << "Error: the value cannot be negative." << std::endl;
            }
        }
        else
        {
            std::cout << "Error: please enter a numeric value." << std::endl;
            std::cin.clear();
        }
        std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');
    }
}

double circleArea(double radius)
{
    return M_PI * radius * radius;
}

int main()
{
    double radius = readDouble("Enter the circle radius: ");

    double area = circleArea(radius);

    std::cout << std::fixed << std::setprecision(6);
    std::cout << "The area of the circle is: " << area << std::endl;

    return 0;
}

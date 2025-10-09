#include <iostream>
#include <limits>
#include <string>

int gcd(int a, int b)
{
    while (b != 0)
    {
        int temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

int readInt(const std::string &prompt)
{
    int value;
    while (true)
    {
        std::cout << prompt;
        if (std::cin >> value)
        {
            return value;
        }
        else
        {
            std::cout << "Error: please enter an integer." << std::endl;
            std::cin.clear();
            std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');
        }
    }
}

int main()
{
    int x = readInt("Enter the first integer: ");
    int y = readInt("Enter the second integer: ");

    std::cout << "GCD = " << gcd(x, y) << std::endl;
    return 0;
}

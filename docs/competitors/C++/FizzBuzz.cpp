#include <iostream>
#include <string>
#include <limits>

std::string fizzBuzz(int n)
{
    if (n % 3 == 0 && n % 5 == 0)
    {
        return "FizzBuzz";
    }
    if (n % 3 == 0)
    {
        return "Fizz";
    }
    if (n % 5 == 0)
    {
        return "Buzz";
    }
    return std::to_string(n);
}

bool readInt(int &value)
{
    if (std::cin >> value)
    {
        return true;
    }
    else
    {
        std::cin.clear();
        std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');
        std::cout << "Error: please enter a valid integer." << std::endl;
        return false;
    }
}

int main()
{
    std::cout << "Enter integers:" << std::endl;
    int number;
    while (true)
    {
        if (readInt(number))
        {
            std::cout << fizzBuzz(number) << std::endl;
        }
        else if (std::cin.eof())
        {
            break;
        }
    }

    return 0;
}

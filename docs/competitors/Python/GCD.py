def gcd(a: int, b: int):

    while b != 0:
        a, b = b, a % b
    return a

def read_int(prompt: str):
    while True:
        try:
            value = int(input(prompt))
            return value
        except ValueError:
            print("Error: please enter an integer.")

def main():
    x = read_int("Enter the first integer: ")
    y = read_int("Enter the second integer: ")

    print(f"GCD = {gcd(x, y)}")

if __name__ == "__main__":
    main()

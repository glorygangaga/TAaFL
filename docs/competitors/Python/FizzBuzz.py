def fizzbuzz(n: int):

    if n % 3 == 0 and n % 5 == 0:
        return "FizzBuzz"
    elif n % 3 == 0:
        return "Fizz"
    elif n % 5 == 0:
        return "Buzz"
    else:
        return str(n)

def main():
    print("Enter integers:")
    try:
        while True:
            line = input()
            if line.strip() == "":
                continue
            try:
                number = int(line)
                print(fizzbuzz(number))
            except ValueError:
                print("Error: please enter a valid integer.")
    except EOFError:
        print("End of input")

if __name__ == "__main__":
    main()

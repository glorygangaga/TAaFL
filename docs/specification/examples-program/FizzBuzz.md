```cpy
func fizzBuzz:str(n:int)
{
    let divisibleBy3:bool = (i % 3 == 0);
    let divisibleBy5:bool = (i % 5 == 0);
    let out:str = "";
    if (divisibleBy3)
    {
        out =  out + "Fizz";
    }
    if (divisibleBy5)
    {
        out = out + "Buzz";
    }
    else
    {
        out = out + toString(i);
    }

    return out;
}

func main:void()
{
    let num:int;
    while(true)
    {
        num = input();
        if (num == 0)
        {
            break;
        }
        print(fizzBuzz(num));
    }
}

```
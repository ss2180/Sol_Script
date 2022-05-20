y = input("Enter a value: ", 0)

x = 0

while(x < 10)
{
    if(x < 4)
    {
        print("if")
    }
    elif(x > 3 and x < 8)
    {
        print("elif")
    }
    else
    {
        print("else")
    }

    x = x + 1
}
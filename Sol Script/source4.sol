
numberOfBottles = input("How many bottles of beer?: ", 1)

while(numberOfBottles > 0)
{
    if(numberOfBottles != 1)
    {
        print(numberOfBottles)
        print(" bottles of beer on the wall, ")

        print(numberOfBottles)
        print(" bottles of beer.\n")
    }
    else
    {
        print(numberOfBottles)
        print(" bottle of beer on the wall, ")

        print(numberOfBottles)
        print(" bottle of beer.\n")
    }


    print("You take one down, pass it around, ")
 
    numberOfBottles = numberOfBottles - 1

    print(numberOfBottles)

    
    if(numberOfBottles == 10)
    {
        print("SURPRISE!SURPRISE!SURPRISE!SURPRISE!")
    }
    elif(numberOfBottles != 1)
    {
        print(" bottles of beer on the wall!\n\n")
    }
    else
    {
        print( " bottle of beer on the wall!\n\n")
    }
}

mylist = list()

x = 0

while(x < 10)
{
    temp = x + 1
    listadd(mylist, temp)
    x = x + 1
}

while(x > 0)
{
    index = x - 1
    temp = listget(mylist, index)
    print(temp)
    print("\n")

    if(x < 5)
    {
        temp2 = x * 2
        listremove(mylist, temp2)
    }

    x = x - 1
}

print("\n\n")

while(x < 6)
{
    if(x == 5)
    {
        listchange(mylist, x, 100)
    }


    temp = listget(mylist, x)
    print(temp)
    print("\n")
    x = x + 1
}
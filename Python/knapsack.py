# citation of knapsack code: https://www.geeksforgeeks.org/printing-items-01-knapsack/?ref=rp
def knapsack(wt, val, W, n, ws=0, nl=[]):
    if n == 0:
        print("There is no items")
        return
    if W <= 0:
        print("No weight for capacity")
        return
    K = [[0 for w in range(W + 1)]
         for i in range(n + 1)]
    # Build table K[][] in bottom
    # up manner
    for i in range(n + 1):
        for w in range(W + 1):
            if i == 0 or w == 0:
                K[i][w] = 0
            elif wt[i - 1] <= w:
                K[i][w] = max(val[i - 1]
                              + K[i - 1][w - wt[i - 1]],
                              K[i - 1][w])
            else:
                K[i][w] = K[i - 1][w]

                # stores the result of Knapsack
    res = K[n][W]
    print("The optimal subset: ", res)

    w = W
    for i in range(n, 0, -1):
        if res <= 0:
            break
        # either the result comes from the
        # top (K[i-1][w]) or from (val[i-1]
        # + K[i-1] [w-wt[i-1]]) as in Knapsack
        # table. If it comes from the latter
        # one/ it means the item is included.
        if res == K[i - 1][w]:
            continue
        else:

            # This item is included.
            ws += wt[i - 1]
            nl.append(i)

            # Since this weight is included
            # its value is deducted
            res = res - val[i - 1]
            w = w - wt[i - 1]
    print("The remaining weight is: ", abs(ws - W))
    print("This is made up from item(s):", nl)


def main():
    """read data from text file, initiate sort method, write output in output file"""
    capacity = ""
    wt = []  # empty list for weight
    val = []  # empty list for value
    print("Please enter a numeric value for the capacity")
    while not capacity.isnumeric():
        capacity = input()
        if not capacity.isnumeric():
            print("Incorrect input! Please enter a numeric value for the capacity.")
    capacity = int(capacity)
    with open("data.txt", 'r') as infile:
        temp = []
        for line in infile:
            if len(line.split()) != 2:
                print("Data out of Bounds")
                return
            for num in line.split():
                temp.append(int(num))
        for i in range(len(temp)):
            if i % 2 == 0:
                wt.append(temp[i])
            else:
                val.append(temp[i])
    n = len(val)
    knapsack(wt, val, capacity, n)


if __name__ == "__main__":
    main()
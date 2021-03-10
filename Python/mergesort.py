def merge_sort(array):
    """take list and divide into two halves"""
    if len(array) <= 1:
        return array
    l, r = merge_sort(array[:int(len(array)/2)]), merge_sort(array[int(len(array)/2):])

    def merge(left, right):
        """compare the right and left sides, and arrange in descending order"""
        results = []  # final output array
        left_p, right_p = 0, 0
        while left_p < len(left) and right_p < len(right):
            if left[left_p] > right[right_p]:
                results.append(left[left_p])
                left_p += 1
            else:
                results.append(right[right_p])
                right_p += 1

        results.extend(left[left_p:])
        results.extend(right[right_p:])
        return results

    return merge(l, r)


def main():
    num_list = []
    final_list = []
    with open("data.txt", 'r') as infile:
        for line in infile:
            for num in line.split():
                num_list.append(int(num))
    range_num = num_list[0]
    if range_num >= len(num_list):
        print("Range out of Bounds")
        return
    for i in range(0, range_num):
        final_list.append(num_list[i+1])

    result = merge_sort(final_list)
    f = open("merge.out", 'w')  # # https://www.w3schools.com/python/python_file_write.asp
    counter = 0
    for n in result:
        f.write(str(n))
        counter += 1
        if counter != range_num:
            f.write(" ")
    f.close()


if __name__ == "__main__":
    main()
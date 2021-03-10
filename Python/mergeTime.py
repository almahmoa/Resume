import time
import random


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
    """use merge method with a list of random number, track time of completion for each set"""
    time_sum = 0
    for i in range(1, 11):   # create a random list in increments 500 for sorting
        for j in range(1, 6):  # create 5 rotations of a random list to generate the average time
            r_list = [random.randint(0, 10000) for _ in range(500 * i)]  # create random list with integer 0<=r<=10000
            tic = time.perf_counter()  # start timer
            merge_sort(r_list)
            toc = time.perf_counter()  # end timer
            time_sum += toc - tic  # overall time
        time_sum = time_sum / 5  # average time
        print(time_sum)


if __name__ == "__main__":
    main()
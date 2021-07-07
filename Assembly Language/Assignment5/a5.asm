TITLE Assignment 5     (a5.asm)

; Author: Abraham Almahmoud
; Last Modified: 5/29/2020
; Course number/section: 271
; Project Number: 5                Due Date: 05/24/2020
; Description: Create a list of 200 random numbers between 10-29. Count the repeated number, and sort them as well. Displays these list.

INCLUDE Irvine32.inc

LO = 10
HI = 29
ARRAYSIZE = 200

.data
intro			BYTE	"Sorting and Counting Radom intergers!		Programmed by Abraham Almahmoud", 10, 0
prompt_1		BYTE	"This program generates 200 random nmbers in the range [10 . . . 29],", 10, "display the original list, sorts the list, displays the median value,", 10, "displays the list sorted in ascending order, then displays the number", 10, "of instance of each generated value.", 10, 10, 0
prompt_2		BYTE	"Your unsorted random numbers:", 10, 0
prompt_3		BYTE	"List Median: ", 0
prompt_4		BYTE	"Your sorted random numbers:", 10, 0
prompt_5		BYTE	"Your list of instances of generated number starting with the number of 10s:", 10, 0
goodbye			BYTE	"Goodbye, and thanks for using my program!", 10, 0
list			DWORD	ARRAYSIZE DUP(?)
count_list		DWORD	20 DUP(?)
space			DWORD	"  ",0
ec_1			BYTE	"**EC: Display the numbers ordered by column instead of by row.", 10, 0
ec_2			BYTE	"**EC: Derive counts before sorting array, then use counts to sort array", 10, 0

.code
main PROC
	push	OFFSET	prompt_1
	push	OFFSET	ec_2
	push	OFFSET	ec_1
	push	OFFSET	intro
	call	introduction

	push	OFFSET	space
	push	OFFSET	prompt_2
	push	HI
	push	LO
	push	OFFSET	list
	push	ARRAYSIZE
	call	fillArray

	push	OFFSET space
	push	OFFSET	prompt_5
	push	HI
	push	LO
	push	OFFSET	count_list
	push	OFFSET	list
	push	ARRAYSIZE
	call	countList

	push	OFFSET	prompt_3
	push	OFFSET	list
	push	ARRAYSIZE
	call	displayMedian

	push	OFFSET	space
	push	OFFSET	prompt_4
	push	LO
	push	OFFSET	list
	push	OFFSET	count_list
	push	ARRAYSIZE
	call	sortList

	push	OFFSET	goodbye
	call	farewell
	exit	; exit to operating system
main ENDP

; ***************************************************************
; Procedure to introduce the user to the program
; receives: Strings
; returns: nothing
; preconditions: Strings are set up
; registers changed: edx
; ***************************************************************
introduction		PROC
	push	ebp
	mov		ebp, esp
	mov		edx, [ebp+8]
	call	WriteString
	mov		edx, [ebp+12]
	call	WriteString
	mov		edx, [ebp+16]
	call	WriteString
	call	CrLf
	mov		edx, [ebp+20]
	call	WriteString
	pop		ebp
	ret		8
introduction		ENDP

; ***************************************************************
; Procedure to fill the array with 200 random numbers (10-29)
; receives: address of array and value of ARRAYSIZE on system stack
; returns: array containing the random numbers
; preconditions: ARRAYSIZE is initialized
; registers changed: eax, ebx, ecx, edi, edx
; ***************************************************************
fillArray			PROC

	push	ebp
	mov		ebp, esp
	mov		ecx, [ebp+8]
	mov		edi, [ebp+12]
	mov		ebx, [ebp+16]
	mov		edx, [ebp+24]
	call	WriteString
	call	Randomize
again:
	mov		eax, [ebp+20]
	sub		eax, ebx
	inc		eax							;set range = HI - LO + 1
	call	RandomRange
	add		eax, ebx
	mov		[edi], eax
	add		edi, 4
	loop	again
	push	[ebp+28]
	push	[ebp+12]
	push	[ebp+8]
	call	displayList
	pop		ebp
	ret		8
fillArray			ENDP

; ***************************************************************
; Procedure to count the amount the same numbers are repeated, and place them in a new array
; receives: address of unsorted array and new array (for counted values)
; returns: new array with filled values
; preconditions: unsorted array is set
; registers changed: eax, ebx, ecx, edi, esi, edx
; ***************************************************************
countList			PROC
	mov		ebx, 0
	push	ebp
	mov		ebp, esp
	mov		esi, [ebp+16]
	mov		eax, [ebp+20]
	mov		edx, [ebp+28]
	call	WriteString
repeat_loop:
	mov		ecx, [ebp+8]
	mov		edi, [ebp+12]
again:
	cmp		[edi], eax
	je		same_value
	jmp		next_array
same_value:
	add		ebx, 1
next_array:
	add		edi, 4
	loop	again
	mov		[esi], ebx
	add		esi, 4
	mov		ebx, 0
	inc		eax
	cmp		eax, [ebp+24]
	jle		repeat_loop
	mov		eax, [ebp+8]
	sub		eax, 180
	push	[ebp+32]
	push	[ebp+16]
	push	eax
	call	displayList
	pop		ebp
	ret		8
countList			ENDP

; ***************************************************************
; Procedure to make an array of sorted numbers
; receives: address of counted array and new array (for sorted values)
; returns: sorted array with filled values
; preconditions: counted array is complete
; registers changed: eax, ebx, ecx, edi, esi, edx
; ***************************************************************
sortList			PROC
	push	ebp
	mov		ebp, esp
	mov		ecx, 20			
	mov		edi, [ebp+12]	
	mov		esi, [ebp+16]	
	mov		eax, [ebp+20]	
	mov		edx, [ebp+24]
	call	WriteString
again:
	mov		ebx, [edi]
repeat_num:
	cmp		ebx, 0
	jle		next_array
	call	exchangeElements
	sub		ebx, 1
	jmp		repeat_num
next_array:
	add		edi, 4
	inc		eax
	loop	again
	push	[ebp+28]
	push	[ebp+16]
	push	[ebp+8]
	call	displayList
	pop		ebp
	ret		8
sortList			ENDP

; ***************************************************************
; Procedure to put numbers into the array. (sorted)
; receives: number via eax
; returns: nothing
; preconditions: eax must be set
; registers changed: esi
; ***************************************************************
exchangeElements	PROC
	mov		[esi], eax
	add		esi, 4
	ret		
exchangeElements	ENDP

; ***************************************************************
; Procedure to display the average of the list
; receives:  address of sorted array and value of ARRAYSIZE on system stack
; returns: none
; preconditions: ARRAYSIZE is initialized, sorted array is filled
; registers changed: eax, ebx, ecx, edi
; ***************************************************************
displayMedian		PROC
	call	CrLf
	push	ebp
	mov		ebp, esp
	mov		ecx, [ebp+8]
	mov		edi, [ebp+12]
	mov		eax, 0
again:
	add		eax, [edi]
	add		edi, 4
	loop	again
;Calculate the (rounded integer) median of the sum.
	mov		ebx, [ebp+8]
	cdq
	div		ebx
	cmp		edx, 100
	jl		round_down
	inc		eax
round_down:	
	mov		edx, [ebp+16]
	call	WriteString
	call	WriteDec
	call	CrLf
	pop		ebp
	ret		8
displayMedian		ENDP

; ***************************************************************
; Procedure to display sorted array
; receives: address of sorted array and value of ARRAYSIZE on system stack
; returns: none
; preconditions: ARRAYSIZE is initialized, sorted array
; registers changed: eax, ebx, ecx, edi. edx
; ***************************************************************
displayList			PROC
	push	ebp
	mov		ebp, esp
	mov		ecx, [ebp+8]
	mov		edi, [ebp+12]
	mov		ebx, edi
	cmp		ecx, 20
	jne		again
display_count:
	mov		eax, [edi]
	call	WriteDec
	mov		edx, [ebp+16]
	call	WriteString
	add		edi, 4
	loop	display_count
	jmp		enddisplay
again:
	
	call	WriteDec
	mov		edx, [ebp+16]
	call	WriteString
	add		edi, 40
	mov		eax, 800
	add		eax, ebx
	cmp		edi, eax
	jge		inc_list
	jmp		continue
inc_list:
	add		ebx, 4
	mov		edi, ebx
	call	CrLf
continue:
	loop	again
enddisplay:
	call	CrLf
	pop		ebp
	ret		12
displayList			ENDP

; ***************************************************************
; Procedure to bid farewell to the user
; receives: strings
; returns: nothing
; preconditions: User's experience must be satisfactory
; registers changed: edx
; ***************************************************************
farewell			PROC
	push	ebp
	mov		ebp, esp
	mov		edx, [ebp+8]
	call	WriteString
	pop		ebp
	ret		8
farewell			ENDP

END main

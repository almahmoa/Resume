TITLE Assignment 6     (a6.asm)

; Author: Abraham Almahmoud
; Last Modified: 06/07/2020
; Course number/section: 271
; Project Number: 6                Due Date: 06/07/2020
; Description: Read 10 inputs from user, valided the inputs as proper integer values (negative, or positive) as string, coverting them to interger.
;			   Convert the 10 numberic values back to string, and display them to the user. Display the sum and average as well.

INCLUDE Irvine32.inc

getString		MACRO		;should display a prompt, then get the user’s keyboard input into a memory location
	displayString
	push	ebp
	push	ecx
	mov		ebp, esp
	mov		edx, [ebp+12]	;address of list in edx
	mov		ecx, 100
	call	ReadString
	mov		esi, edx
	pop		ecx
	pop		ebp
ENDM

displayString	MACRO		;should print the string which is stored in a specified memory location.
	push	ebp
	push	edx				;Save edx register
	mov		ebp, esp
	mov		edx, [ebp+8]
	call	WriteString
	pop		edx				;Restore edx
	pop		ebp				;Restore stack
ENDM

.data
title_1		BYTE	"PROGRAMMING ASSIGNMENT 6: Designing low-level I/O prodocedures" , 0
author_1	BYTE	"Written by: Abraham Almahmoud", 0
prompt_1	BYTE	"Please provide 10 signed decimal integers.", 0
prompt_2	BYTE	"Each number needs to be small enough to fit inside a 32 bit register.", 0
prompt_3	BYTE	"After you have finished inputting the raw numbers I will display a list", 10, "of the integers, their sum, and their average value.", 0
input_1		BYTE	"Please enter an signed number: ", 0
input_2		BYTE	"ERROR: you did not enter a signed number or your number was too big.", 10, "Please try again: ", 0
prompt_4	BYTE	"You entered the folowing numbers:", 10, 0
prompt_5	BYTE	"The sum of these numbers is: ", 0
prompt_6	BYTE	"The rounded average is: ", 0
prompt_7	BYTE	"Thanks for playing!", 0
in_list		DWORD	10000 DUP(0)
out_list	DWORD	10000 DUP(0)

.code
main PROC
	push	OFFSET title_1
	displayString
	call	CrLf
	push	OFFSET author_1
	displayString
	call	CrLf
	call	CrLf
	push	OFFSET prompt_1
	displayString
	call	CrLf
	push	OFFSET prompt_2
	displayString
	call	CrLf
	push	OFFSET prompt_3
	displayString
	call	CrLf
	call	CrLf
	push	OFFSET in_list		;16
	push	OFFSET input_2		;12
	push	OFFSET input_1		;8
	call	ReadVal
	call	CrLf
	push	OFFSET prompt_6		;24
	push	OFFSET prompt_5		;20
	push	OFFSET prompt_4		;16
	push	OFFSET out_list		;12
	push	OFFSET in_list		;8
	call	WriteVal
	call	CrLf
	call	CrLf
	push	OFFSET prompt_7
	displayString
	call	CrLf
	exit	; exit to operating system
main ENDP

; ***************************************************************
; Procedure to read inputs from user, verifying if correct, and storing 10 total inputs as numeric values in in_list
; receives: empty list, and prompts
; returns: User input stored in in_list as numeric values
; preconditions: None
; registers changed: edx, esi, edi, eax, ebx, ecx
; ***************************************************************
ReadVal			PROC		;should invoke the getString macro to get the user’s string of digits. It should then convert the digit string to numeric, while validating the user’s input.
	push	ebp
	mov		ebp, esp
	mov		edx, [ebp+8]
	mov		edi, [ebp+16]
	mov		ebx, 10

read_next_input:
	push	edi
	push	edx
	getString				;reads input from user, store into memory
	push	ebx				;store checker for 10 loops
	mov		ebx, eax
	mov		eax, edi
	mov		ecx, ebx		;move string length to ecx
	mov		ebx, 0
	push	edi				;store memory location
	lodsb					;check first run
	cmp		al,43			; '+' is character 43
	je		signed
	cmp		al,45			; '-' is character 45
	je		neg_signed
	jmp		numbers
signed:
	mov		ebx, 1
	dec		ecx
	jmp		counter
neg_signed:
	mov		ebx, 2
	dec		ecx

counter:					;check current memory in esi
	lodsb
numbers:
	cmp		al,48			; '0' is character 48
	jl		not_valid
	cmp		al,57			; '9' is character 57
	jg		not_valid
	stosb
signed_save:
	loop	counter

	mov		ecx, 1
	call	ParseDecimal32		;converts string to numeric
	jc		not_valid			;jump if carry flag is set
	cmp		ebx, 0
	je		writer
	push	ebx					;store sign checker
	mov		ebx, 10				;if sign was used, divide by ten to remove last repeat digit
	cdq
	div		ebx
	pop		ebx					;restore sign checker
	cmp		ebx, 1
	je		writer
	mov		ebx, -1				;negative converting
	mul		ebx

writer:							;places numeric inside in_list [ebp+16]
	pop		edi					;restore memory location
	mov		[edi], eax
	add		edi, 4
	pop		ebx					;restore loop checker
	mov		eax, ebx
	dec		ebx
	cmp		ebx, 0
	je		end_read
	mov		edx, [ebp+8]
	jmp		read_next_input		;request next input

not_valid:
	pop		edi
	pop		ebx
	mov		eax, ebx
	pop		edx
	pop		edi
	mov		edx, [ebp+12]
	jmp		read_next_input

end_read:
	mov		ecx, 10
loop_stack:						;retore excessive memory
	pop		edx
	pop		edi
	loop	loop_stack
	pop		ebp
	ret		8
ReadVal		ENDP

; ***************************************************************
; Procedure to convert the numberic values to string. Sum the numberic values, and provide the average
; receives: in_list, out_list, and prompts
; returns: nothing
; preconditions: in_list must be set up with the inital 10 numeric values from ReadVal
; registers changed: edx, eax, esi, edi, ebx, ecx
; ***************************************************************
WriteVal	PROC			;should convert a numeric value to a string of digits, and invoke the displayString macro to produce the output.
	push	ebp
	mov		ebp, esp
	push	[ebp+16]		;prompt_4
	displaystring
	pop		edi
	mov		esi, [ebp+8]	;inlist
	mov		edi, [ebp+12]	;outlist
	mov		ebx, 3			;counter for next_loop to end
	push	ebx
	mov		ebx, 10			;counter for display list

loop_writer:				;loop that converts numeric to string
	push	ebx				;counter for loops
	mov		ecx, 0
	mov		eax, [esi]		;move memory value to eax
	cmp		eax, 0			;check if interger is negative
	jge		set_divider
	mov		ebx, -1
	mul		ebx				;convert negative value to positive

set_divider:
	mov		ebx, 10
loop_num_to_string:
	mov		edx, 0
	div		ebx
	add		edx, 48
	push	edx
	inc		ecx
	cmp		eax, 0
	jne		loop_num_to_string
	mov		ebx, 0

;adds minus signs if negative to the beginning of string
	mov		eax, [esi]
	cmp		eax, 0			;check if interger is negative
	jge		loop_add_to_mem
	mov		edx, 45
	push	edx
	inc		ecx
	mov		ebx, 0

loop_add_to_mem:
	pop		[edi]			;store value in mem
	inc		edi				;increment location space
	inc		ebx				;increment number of 
	loop	loop_add_to_mem

	sub		edi, ebx		;set memory back to start of string
	mov		ebx, 0
	push	edi				;push mem location
	displayString

	pop		edi				;restore mem location
	add		esi, 4
	pop		ebx
	dec		ebx
	cmp		ebx, 0
	je		next_loop		;after the 10 inital numberic values have been completed, move to next objective (sum, and average)

	mov		edx, 44			;',' is character 44
	push	edx
	pop		[edi]
	push	edi
	displayString			;display ','
	pop		edi
	mov		edx, 32			;' ' is character 32
	push	edx
	pop		[edi]
	push	edi
	displayString			;display ' '
	pop		edi
	jmp		loop_writer

next_loop:
	pop		ebx				;counter for avg_num and end
	dec		ebx
	cmp		ebx, 0
	jle		end_of_code
	cmp		ebx, 1
	je		avg_num
	push	ebx				;store counter for end ebx = 2

	call	crlf
	push	[ebp+20]		;prompt_5
	displaystring
	pop		edi
	mov		edi, [ebp+12]	;outlist

	mov		ebx, 0
	sub		esi, 40			;restore esi to base position
	mov		ecx, 10	
sum_num:					;sum the 10 numbers in the in_list
	mov		eax, [esi]
	add		ebx, eax
	add		esi, 4
	loop	sum_num
	mov		[esi], ebx		;place the sum in the last position of esi
	mov		ebx, 1
	jmp		loop_writer		;loop num to string operation with a counter loop of 1

avg_num:
	push	ebx				;store for end ebx = 1
	call	crlf
	push	[ebp+24]		;prompt_6
	displaystring
	pop		edi
	mov		edi, [ebp+12]	;outlist
	cmp		eax, 0
	jge		divide_num
	mov		ebx, -1
	mul		ebx
	mov		ecx, 1

divide_num:					;divide the numeric sum by 10
	mov		ebx, 10
	cdq
	div		ebx
	cmp		ecx, 1
	jne		pass_num
	mov		ebx, -1
	mul		ebx
pass_num:					;store the average numeric value in esi
	mov		[esi], eax
	mov		ebx, 1
	jmp		loop_writer		;loop once to output average as string
end_of_code:
	pop		ebp
	ret		8
WriteVal	ENDP
END main

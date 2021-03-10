# Author: Abraham Almahmoud
# Date: 03/02/2020
# Description: A simple game of XiangQi.


class Piece:
    """This class holds main functions for pieces on the board"""
    def __init__(self, name, color):
        self.name = name
        self.color = color

    def is_valid(self, start_pos, end_pos, board):
        """check if movement is valid for the unit"""
        if self.valid_move(start_pos[0], start_pos[1], end_pos[0], end_pos[1], board):
            return True
        return False

    def move_space(self, from_row, to_row, from_col, to_col, board, move, battle):
        """check if movement path for units are valid"""
        # orthogonally movements
        if move == "right":
            cur_space = from_col + 1
            while cur_space < to_col:
                if board[from_row][cur_space] == 1:
                    cur_space += 1
                else:
                    if cur_space != to_col and self.name == "c" and not battle:  # cannon jumped
                        return self.move_space(from_row, to_row, cur_space, to_col, board, "right", True)
                    return False
            if board[from_row][cur_space] != 1:
                for unit in board[from_row][cur_space]:
                    if cur_space == to_col and unit.color != self.color:
                        return battle
                return False
            if self.name == "c" and battle:
                return False
            return True
        if move == "left":
            cur_space = from_col - 1
            while cur_space > to_col:
                if board[from_row][cur_space] == 1:
                    cur_space -= 1
                else:
                    if cur_space != to_col and self.name == "c" and not battle:  # cannon jumped
                        return self.move_space(from_row, to_row, cur_space, to_col, board, "left", True)
                    return False
            if board[from_row][cur_space] != 1:
                for unit in board[from_row][cur_space]:
                    if cur_space == to_col and unit.color != self.color:
                        return battle
                return False
            if self.name == "c" and battle:
                return False
            return True
        if move == "down":
            cur_space = from_row + 1
            while cur_space < to_row:
                if board[cur_space][from_col] == 1:
                    cur_space += 1
                else:
                    if cur_space != to_row and self.name == "c" and not battle:  # cannon jumped
                        return self.move_space(cur_space, to_row, from_col, to_col, board, "down", True)
                    return False
            if board[cur_space][from_col] != 1:
                for unit in board[cur_space][from_col]:
                    if cur_space == to_row and unit.color != self.color:
                        return battle
                return False
            if self.name == "c" and battle:
                return False
            return True
        if move == "up":
            cur_space = from_row - 1
            while cur_space > to_row:
                if board[cur_space][from_col] == 1:
                    cur_space -= 1
                else:
                    if cur_space != to_row and self.name == "c" and not battle:  # cannon jumped
                        return self.move_space(cur_space, to_row, from_col, to_col, board, "up", True)
                    return False
            if board[cur_space][from_col] != 1:
                for unit in board[cur_space][from_col]:
                    if cur_space == to_row and unit.color != self.color:
                        return battle
                return False
            if self.name == "c" and battle:
                return False
            return True
        # diagonally movements
        if move == "up_right":
            cur_space = from_col + 1
            from_row -= 1
            while cur_space < to_col:
                if board[from_row][cur_space] == 1:
                    cur_space += 1
                    from_row -= 1
                else:
                    return False
            if board[from_row][cur_space] != 1:
                for unit in board[from_row][cur_space]:
                    if cur_space == to_col and unit.color != self.color:
                        return battle
                return False
            return True
        if move == "down_right":
            cur_space = from_col + 1
            from_row += 1
            while cur_space < to_col:
                if board[from_row][cur_space] == 1:
                    cur_space += 1
                    from_row += 1
                else:
                    return False
            if board[from_row][cur_space] != 1:
                for unit in board[from_row][cur_space]:
                    if cur_space == to_col and unit.color != self.color:
                        return battle
                return False
            return True
        if move == "up_left":
            cur_space = from_row - 1
            from_col -= 1
            while cur_space > to_row:
                if board[cur_space][from_col] == 1:
                    cur_space -= 1
                    from_col -= 1
                else:
                    return False
            if board[cur_space][from_col] != 1:
                for unit in board[cur_space][from_col]:
                    if cur_space == to_row and unit.color != self.color:
                        return battle
                return False
            return True
        if move == "down_left":
            cur_space = from_row + 1
            from_col -= 1
            while cur_space < to_row:
                if board[cur_space][from_col] == 1:
                    cur_space += 1
                    from_col -= 1
                else:
                    return False
            if board[cur_space][from_col] != 1:
                for unit in board[cur_space][from_col]:
                    if cur_space == to_row and unit.color != self.color:
                        return battle
                return False
            return True


class General(Piece):
    """
    The general starts the game at the midpoint of the back edge, within the palace. The general may move and capture
    one point orthogonally and may not leave the palace.
    """
    def valid_move(self, from_row, from_col, to_row, to_col, board):
        color = self.color
        # palace
        if color == 'red':
            if to_col <= 2 or to_col >= 6 or to_row >= 3:
                return False
        if color == 'black':
            if to_col <= 2 or to_col >= 6 or to_row <= 6:
                return False
        # orthogonal movement
        if from_col + 1 == to_col and from_row == to_row:
            return self.move_space(from_row, to_row, from_col, to_col, board, "right", True)
        if from_col - 1 == to_col and from_row == to_row:
            return self.move_space(from_row, to_row, from_col, to_col, board, "left", True)
        if from_row + 1 == to_row and from_col == to_col:
            return self.move_space(from_row, to_row, from_col, to_col, board, "down", True)
        if from_row - 1 == to_row  and from_col == to_col:
            return self.move_space(from_row, to_row, from_col, to_col, board, "up", True)
        return False


class Advisor(Piece):
    """
    The advisors start on either side of the general. They move and capture one point diagonally and may not leave the
    palace, which confines them to five points on the board.
    """
    def valid_move(self, from_row, from_col, to_row, to_col, board):
        color = self.color
        # palace
        if color == 'red':
            if to_col <= 2 or to_col >= 6 or to_row >= 3:
                return False
        if color == 'black':
            if to_col <= 2 or to_col >= 6 or to_row <= 6:
                return False
        # diagonal movements
        if to_row == from_row - 1 and to_col == from_col + 1:
            return self.move_space(from_row, to_row, from_col, to_col, board, "up_right", True)
        if to_row == from_row + 1 and to_col == from_col + 1:
            return self.move_space(from_row, to_row, from_col, to_col, board, "down_right", True)
        if to_row == from_row - 1 and to_col == from_col - 1:
            return self.move_space(from_row, to_row, from_col, to_col, board, "up_left", True)
        if to_row == from_row + 1 and to_col == from_col - 1:
            return self.move_space(from_row, to_row, from_col, to_col, board, "down_left", True)
        return False


class Elephant(Piece):
    """
    They are located next to the advisors. These pieces move and capture exactly two points diagonally and may not
    jump over intervening pieces. Elephants may not cross the river, and serve as defensive pieces. Because an
    elephant's movement is restricted to just seven board positions.
    """
    def valid_move(self, from_row, from_col, to_row, to_col, board):
        color = self.color
        # crossing river
        if color == 'red':
            if to_row >= 6:
                return False
        else:
            if to_row <= 5:
                return False
        # diagonal movements
        if to_row == from_row - 2 and to_col == from_col + 2:
            return self.move_space(from_row, to_row, from_col, to_col, board, "up_right", True)
        if to_row == from_row + 2 and to_col == from_col + 2:
            return self.move_space(from_row, to_row, from_col, to_col, board, "down_right", True)
        if to_row == from_row - 2 and to_col == from_col - 2:
            return self.move_space(from_row, to_row, from_col, to_col, board, "up_left", True)
        if to_row == from_row + 2 and to_col == from_col - 2:
            return self.move_space(from_row, to_row, from_col, to_col, board, "down_left", True)
        return False


class Horse(Piece):
    """
     Horses begin the game next to the elephants, on their outside flanks. A horse moves and captures one point
     orthogonally and then one point diagonally away from its former position, The horse does not jump as the knight
     does in Western chess, and can be blocked by a piece located one point horizontally or vertically adjacent to it.
    """
    def valid_move(self, from_row, from_col, to_row, to_col, board):
        valid_location = [(from_row - 2, from_col + 1), (from_row - 1, from_col + 2), (from_row + 1, from_col + 2),
                          (from_row + 2, from_col + 1), (from_row + 2, from_col - 1), (from_row + 1, from_col - 2),
                          (from_row - 1, from_col - 2), (from_row - 2, from_col - 1), (from_row - 2, from_col + 1)]
        if (to_row, to_col) in valid_location:
            if from_row + 2 == to_row:
                if self.move_space(from_row, from_row + 1, from_col, from_col, board, "down", False):
                    if from_col < to_col:
                        return self.move_space(from_row + 1, to_row, from_col, to_col, board, "down_right", True)
                    else:
                        return self.move_space(from_row + 1, to_row, from_col, to_col, board, "down_left", True)
            if from_row - 2 == to_row:
                if self.move_space(from_row, from_row - 1, from_col, from_col, board, "up", False):
                    if from_col < to_col:
                        return self.move_space(from_row - 1, to_row, from_col, to_col, board, "up_right", True)
                    else:
                        return self.move_space(from_row - 1, to_row, from_col, to_col, board, "up_left", True)
            if from_col - 2 == to_col:
                if self.move_space(from_row, from_row, from_col, from_col - 1, board, "left", False):
                    if from_row > to_row:
                        return self.move_space(from_row, to_row, from_col - 1, to_col, board, "up_left", True)
                    else:
                        return self.move_space(from_row, to_row, from_col - 1, to_col, board, "down_left", True)
            if from_col + 2 == to_col:
                if self.move_space(from_row, from_row, from_col, from_col + 1, board, "right", False):
                    if from_row > to_row:
                        return self.move_space(from_row, to_row, from_col + 1, to_col, board, "up_right", True)
                    else:
                        return self.move_space(from_row, to_row, from_col + 1, to_col, board, "down_right", True)
        return False


class Chariot(Piece):
    """
     The chariot moves and captures any distance orthogonally, but may not jump over intervening pieces.
     The chariots begin the game on the points at the corners of the board.
    """
    def valid_move(self, from_row, from_col, to_row, to_col, board):
        if from_row == to_row:
            if from_col < to_col:
                return self.move_space(from_row, to_row, from_col, to_col, board, "right", True)
            if to_col < from_col:
                return self.move_space(from_row, to_row, from_col, to_col, board, "left", True)
        if from_col == to_col:
            if from_row < to_row:
                return self.move_space(from_row, to_row, from_col, to_col, board, "down", True)
            if to_row < from_row:
                return self.move_space(from_row, to_row, from_col, to_col, board, "up", True)
        return False


class Cannon(Piece):
    """
    Each player has two cannons, which start on the row behind the soldiers, two points in front of the horses.
    Cannons move like chariots, any distance orthogonally without jumping, but can only capture by jumping a
    single piece, friend or foe, along the path of attack. Cannons can be exchanged for horses immediately
    from their starting positions.
    """
    def valid_move(self, from_row, from_col, to_row, to_col, board):
        if from_row == to_row:
            if from_col < to_col:
                return self.move_space(from_row, to_row, from_col, to_col, board, "right", False)
            if to_col < from_col:
                return self.move_space(from_row, to_row, from_col, to_col, board, "left", False)
        if from_col == to_col:
            if from_row < to_row:
                return self.move_space(from_row, to_row, from_col, to_col, board, "down", False)
            if to_row < from_row:
                return self.move_space(from_row, to_row, from_col, to_col, board, "up", False)
        return False


class Soldier(Piece):
    """
    Each side starts with five soldiers. Soldiers begin the game located on every other point one row back from the
    edge of the river. They move and capture by advancing one point. Once they have crossed the river, they may also
     move and capture one point horizontally. Soldiers cannot move backward, and therefore cannot retreat; after
     advancing to the last rank of the board, however, a soldier may still move sidemoves at the enemy's edge.
     """
    def valid_move(self, from_row, from_col, to_row, to_col, board):
        color = self.color
        if color == 'red':
            if from_row + 1 == to_row and from_col == to_col:
                return self.move_space(from_row, to_row, from_col, to_col, board, "down", True)
            if from_row >= 5 and (from_row + 1 == to_row or from_row == to_row):  # check if soldier has crossed the river
                if from_col + 1 == to_col:
                    return self.move_space(from_row, to_row, from_col, to_col, board, "right", True)
                if from_col - 1 == to_col:
                    return self.move_space(from_row, to_row, from_col, to_col, board, "left", True)
        if color == 'black':
            if from_row - 1 == to_row and from_col == to_col:
                return self.move_space(from_row, to_row, from_col, to_col, board, "up", True)
            if from_row <= 4 and (from_row - 1 == to_row or from_row == to_row):  # check if soldier has crossed the river
                if from_col + 1 == to_col:
                    return self.move_space(from_row, to_row, from_col, to_col, board, "right", True)
                if from_col - 1 == to_col:
                    return self.move_space(from_row, to_row, from_col, to_col, board, "left", True)
        return False


class XiangqiGame:
    """This will run the game using pieces from Piece()"""
    def __init__(self):
        """sets up board, player's color, and game state"""
        self.board = [[Chariot("r", 'red')], [Horse("h", 'red')], [Elephant("e", 'red')],
                      [Advisor("a", 'red')], [General("g", 'red')], [Advisor("a", 'red')],
                      [Elephant("e", 'red')], [Horse("h", 'red')], [Chariot("r", 'red')]], \
                     [1, 1, 1, 1, 1, 1, 1, 1, 1], \
                     [1, [Cannon("c", 'red')], 1, 1, 1, 1, 1, [Cannon("c", 'red')], 1], \
                     [[Soldier("s", 'red')], 1, [Soldier("s", 'red')], 1, [Soldier("s", 'red')],
                      1, [Soldier("s", 'red')], 1, [Soldier("s", 'red')]], \
                     [1, 1, 1, 1, 1, 1, 1, 1, 1], \
                     [1, 1, 1, 1, 1, 1, 1, 1, 1], \
                     [[Soldier("s", 'black')], 1, [Soldier("s", 'black')], 1, [Soldier("s", 'black')], 1,
                      [Soldier("s", 'black')], 1, [Soldier("s", 'black')]], \
                     [1, [Cannon("c", 'black')], 1, 1, 1, 1, 1, [Cannon("c", 'black')], 1], \
                     [1, 1, 1, 1, 1, 1, 1, 1, 1], \
                     [[Chariot("r", 'black')], [Horse("h", 'black')], [Elephant("e", 'black')],
                      [Advisor("a", 'black')], [General("g", 'black')], [Advisor("a", 'black')],
                      [Elephant("e", 'black')], [Horse("h", 'black')],  [Chariot("r", 'black')]]
        self.player_color = 'red'
        self.game_state = 'UNFINISHED'
        self.temp_unit = []

    def get_game_state(self):
        """returns current game state"""
        return self.game_state

    def make_move(self, pos1, pos2):
        """starts the function that will progress the game, moving units and checking game status"""
        if self.game_state != 'UNFINISHED':
            if self.game_state == "DRAW":
                print("Game Over -- No Winner")
            else:
                print("Game Over -- ", self.game_state)
            return True

        alpha_to_index = {  # convert alpha values to int
            "a": 0,
            "b": 1,
            "c": 2,
            "d": 3,
            "e": 4,
            "f": 5,
            "g": 6,
            "h": 7,
            "i": 8
        }
        if pos1[0].lower() in alpha_to_index and 1 <= int(pos1[1:]) <= 10 and \
                pos2[0].lower() in alpha_to_index and 1 <= int(pos2[1:]) <= 10:  # check if input is valid
            # start position
            start_row = int(pos1[1:])-1
            start_col = alpha_to_index[pos1[0].lower()]
            # end position
            end_row = int(pos2[1:])-1
            end_col = alpha_to_index[pos2[0].lower()]
            if self.board[start_row][start_col] != 1:  # check if start position is unit
                for unit in self.board[start_row][start_col]:
                    if unit.color != self.player_color:  # check if player is moving their unit
                        print("it is ", self.player_color, "'s turn")
                        return False
                    if unit.is_valid([start_row, start_col], [end_row, end_col], self.board):  # check if move is valid
                        if not self.is_in_check(self.player_color):  # check if player put themselves in check
                            self.board[end_row][end_col] = self.board[start_row][start_col]  # move unit to location
                            self.board[start_row][start_col] = 1  # past location in set to 1
                            if self.is_draw():  # check if no units are left other than advisor and general
                                return False
                            if self.player_color == 'red':
                                self.player_color = 'black'
                            else:
                                self.player_color = 'red'
                            self.checkmate(self.player_color)
                        else:
                            print("invalid move, you're in check")
                    else:
                        print("invalid move")
            else:
                print("no units in this square")
        else:
            print("invalid input")
        return True

    def is_in_check(self, color):
        """check if the general is in check"""
        g_pos = self.general_position(color)  # find current location of general
        for cur_row in range(0, 10):
            for cur_col in range(0, 9):
                if self.board[cur_row][cur_col] != 1:
                    for unit in self.board[cur_row][cur_col]:
                        if unit.color != color:  # for opposing units
                            if unit.is_valid([cur_row, cur_col], g_pos, self.board):  # check if enemy's reach general
                                return True
        return False

    def checkmate(self, color):
        """check if the player in in checkmate, or has no move valid moves"""
        for cur_row in range(0, 10):
            for cur_col in range(0, 9):
                if self.board[cur_row][cur_col] != 1:
                    for unit in self.board[cur_row][cur_col]:
                        if unit.color == color:  # for current players units
                            for nxt_row in range(0, 10):
                                for nxt_col in range(0, 9):
                                    if unit.is_valid([cur_row, cur_col], [nxt_row, nxt_col], self.board):
                                        # store unit in valid temp space for checker
                                        self.temp_space(nxt_row, nxt_col, cur_row, cur_col, False)
                                        if not self.is_in_check(color):  # check if still in 'check' at new space
                                            # return unit back to original space before returning to code
                                            self.temp_space(cur_row, cur_col, nxt_row, nxt_col, True)
                                            return  # a valid movement prevents check
                                        # return unit back to original space before returning to the 'for' loop
                                        self.temp_space(cur_row, cur_col, nxt_row, nxt_col, True)
                                    pass  # no valid space was found for unit
        if self.player_color != 'red':
            if self.is_in_check(color):
                print("checkmate--red wins")
            else:  # if general is not in check, but no valid moves exist that wont be in check
                print("stalemate--red wins")
            self.game_state = "RED_WON"
        else:
            if self.is_in_check(color):
                print("checkmate--black won")
            else:
                print("stalemate--black wins")
            self.game_state = "BLACK_WON"
        return

    def is_draw(self):
        """check if players cannot checkmate each other, only advisors and/or generals left on board"""
        units = ["r", "h", "e", "c", "s"]  # all other units
        player_has_unit = False
        opponent_has_unit = False
        for cur_row in range(0, 10):
            for cur_col in range(0, 9):
                if self.board[cur_row][cur_col] != 1:
                    for unit in self.board[cur_row][cur_col]:
                        if unit.name in units and unit.color == self.player_color:  # if they have other units
                            player_has_unit = True
                        if unit.name in units and unit.color != self.player_color:
                            opponent_has_unit = True
        if player_has_unit or opponent_has_unit:  # check if either players has more than advisors or generals
            return False
        self.game_state = "DRAW"
        return True

    def general_position(self, color):
        """find your general's current location"""
        for cur_row in range(0, 10):
            for cur_col in range(0, 9):
                if self.board[cur_row][cur_col] != 1:
                    for unit in self.board[cur_row][cur_col]:
                        if unit.name == "g" and unit.color == color:  # if unit is current player's general
                            return [cur_row, cur_col]

    def temp_space(self, row1, col1, row2, col2, revert):
        """a function which will switch a position of a unit for checkmate checker"""
        if self.board[row1][col1] != 1:
            self.temp_unit = self.board[row1][col1]
        self.board[row1][col1] = self.board[row2][col2]
        if self.temp_unit == [] or not revert:
            self.board[row2][col2] = 1
        else:
            self.board[row2][col2] = self.temp_unit
            self.temp_unit = []
        return

    def print_board(self):
        """prints the board"""
        print("   |     A      |      B     |     C      |     D      |      E     |"
              "     F      |     G      |      H     |      I     |")
        for cur_row in range(0, 10):
            print("_" * 121)
            if cur_row < 9:
                print(chr(cur_row+1 + 48), end="  | ")
            elif cur_row == 9:
                print("10", end=" | ")
            for cur_col in range(0, 9):
                if self.board[cur_row][cur_col] == 1:
                    panel = "(        )"
                else:
                    for space in self.board[cur_row][cur_col]:
                        panel = (space.name, space.color[0])
                print(str(panel) + ' | ', end="")
            print()
        print("_" * 121)
        return

    def play(self):
        """run the game with user inputs"""
        self.print_board()
        while self.game_state == 'UNFINISHED':
            print(self.player_color, "'s turn")
            print("1st coordinate")
            pos1 = str(input())
            print("2nd coordinate")
            pos2 = str(input())
            self.make_move(pos1, pos2)
            self.print_board()
        return


game = XiangqiGame()
game.print_board()
game.make_move('b3', 'b7')
game.make_move('e7', 'e6')
game.make_move('h3', 'h6')
game.make_move('b8', 'b1')
game.print_board()
# game.make_move('h8', 'h1')
#game.make_move('b7', 'e7')
#game.make_move('c10', 'a8')
# game.make_move('a1', 'a2')
# game.make_move('d10', 'e9')
#game.make_move('h6', 'e6')
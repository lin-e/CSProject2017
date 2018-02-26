from turtle import *
import math # used for the ceiling calculations
import random # used to generate weights for each node, instead of weighting the arcs
import datetime # used to measure time difference

class grid: # grid to hold two dimensional points
    def __init__(self, width, height): # creates new grid with specified dimensions
        self.grid_width = width # sets the width
        self.grid_height = height # sets height
        self.main_grid = list() # declares new list
        for i in range(0, self.grid_height): # for every row
            self.main_grid.append([0] * self.grid_width) # creates an array with the same size as the width
    def display(self): # displays the grid
        for y in self.main_grid: # goes through each row
            item_string = "" # makes empty string
            for x in y: # goes through each item in the row
                item_string += str(x) # appends to the string (these were designed with numbers, hence the string conversion)
            print(item_string) # displays the line
    def set_point(self, x, y, item): # sets the point on the grid
        self.main_grid[y][x] = item # ''
    def get_point(self, x, y): # gets the point from the grid
        return self.main_grid[y][x] # ''
class maze: # holds all the data for the maze
    def __init__(self, parent_grid, upper): # creates a new maze, with a parent and an upper bound on randomness
        self.parent = parent_grid # sets its parent
        self.upper_bound = upper # sets its upper bound
        self.grid_width = self.parent.grid_width - 2 # calculates it with a margin of 1
        self.grid_height = self.parent.grid_height - 2 # ''
        self.node_width = math.ceil(self.grid_width / 2) # calculates the number of nodes needed
        self.node_height = math.ceil(self.grid_height / 2) # ''
        self.node_grid = grid(self.node_width, self.node_height) # creates a new grid to store node weights
        self.active_nodes = list() # creates a list of active nodes, used below
        self.dead_nodes = list() # creates a list of dead nodes, used below
        for x in range(0, self.node_width): # iterates for each column in the node grid
            for y in range(0, self.node_height): # iterates for each row in the node grid
                self.node_grid.set_point(x, y, random.randint(1, self.upper_bound)) # sets the point in the node weights with a pseudo-random number
    def move_point(self, origin, direction): # function to move points, for cleaner code
        if (direction == 1): # if the direction is up
            return [origin[0], origin[1] - 1] # changes y by -1 (the top-left corner is 0,0)
        if (direction == 2): # if the direction is down
            return [origin[0], origin[1] + 1] # changes y by 1
        if (direction == 3): # if the direction is left
            return [origin[0] - 1, origin[1]] # changes x by -1
        if (direction == 4): # if the direction is right
            return [origin[0] + 1, origin[1]] # changes x by 1
        return origin # otherwise, returns the original point
    def get_export_point(self, point): # gets the point a node would represent on the parent grid
        return [(point[0] * 2) + 1, (point[1] * 2) + 1] # maps the point onto the parent grid
    def get_surrounding(self, node): # gets points surrounding a set point
        possible_points = list() # empty list to hold new points
        directions = [1, 2, 3, 4] # array of all directions
        direction_skip = list() # list to hold directions that should be skipped
        if (node[0] == 0): # if the node is on the left of the grid
            direction_skip.append(3) # skips left
        if (node[0] == self.node_width - 1): # if the node is on the right of the grid
            direction_skip.append(4) # skips right
        if (node[1] == 0): # if the node is on the top of the grid
            direction_skip.append(1) # skips up
        if (node[1] == self.node_height - 1): # if the node is on the bottom of the grid
            direction_skip.append(2) # skips down
        for single_direction in directions: # goes through every direction
            if single_direction in direction_skip: # if the direction should be skipped
                continue # continues to next iteration
            new_point = self.move_point(node, single_direction) # the new point is created with the move function
            if new_point in self.active_nodes: # if the point is already active
                continue # continues to next iteration
            possible_points.append(new_point) # adds the new point onto the list
        if len(possible_points) == 0: # if the new points list is empty
            self.dead_nodes.append(node) # declares a set node as dead
        return possible_points # returns the list of all possible points
    def generate(self): # main function to generate a new maze
        start_point = [0, 0] # sets the start point as the top left corner of the node grid
        self.active_nodes.append(start_point) # adds the new point to the active list
        while len(self.active_nodes) < (self.node_width * self.node_height): # keeps looping until every point is active (such that prims algorithm has created a minimum spanning tree)
            possible_arcs = list() # creates a new list of possible arcs
            for single_node in self.active_nodes: # goes through every active node
                if single_node in self.dead_nodes: # if the node is marked as dead
                    continue # continues to next iteration
                for possible_node in self.get_surrounding(single_node): # goes through each node surrounding the current active node
                    possible_arcs.append(arc(self, single_node, possible_node)) # creates a new arc and adds it to the list
            current_arc = None # null placeholder for the current arc
            current_min = self.upper_bound + 2 # sets the current minimum distance to be two more than the upper bound, to ensure it will be an actual arc
            for single_arc in possible_arcs: # goes through every possible arc
                if single_arc.node_distance < current_min: # if the arc's distance is less than the current minimum
                    current_min = single_arc.node_distance # the current minimum is set to the current arc's minimum
                    current_arc = single_arc # the current arc is set to the arc used
            self.active_nodes.append(current_arc.end) # adds the end point of the arc as an active node
            actual_point_start = self.get_export_point(current_arc.start) # maps the point onto the parent grid
            actual_point_path = self.move_point(actual_point_start, current_arc.move_dir) # ''
            actual_point_end = self.get_export_point(current_arc.end) # ''
            self.parent.set_point(actual_point_start[0], actual_point_start[1], 1) # marks the point as a path
            self.parent.set_point(actual_point_path[0], actual_point_path[1], 1) # ''
            self.parent.set_point(actual_point_end[0], actual_point_end[1], 1) # ''
            #print((self.node_width * self.node_height) - len(self.active_nodes))
    def display(self, wall): # displays the grid
        for y in range(0, self.parent.grid_height): # goes through each row
            final_line = "" # creates an empty string to hold row
            for x in range(0, self.parent.grid_width): # goes through each item in the row
                if self.parent.get_point(x, y) == 0: # checks if the point is a wall
                    final_line += wall # adds a wall character
                else: # otherwise
                    final_line += " " # adds an empty space
            print(final_line) # prints the current row as a line
class arc: # class to hold information between two points
    def __init__(self, parent_maze, start_node, end_node): # creates a new arc with a parent and two points
        self.start = start_node # sets the start node
        self.end = end_node # sets the end node
        start_weight = parent_maze.node_grid.get_point(start_node[0], start_node[1]) # gets the weight of the start node
        end_weight = parent_maze.node_grid.get_point(end_node[0], end_node[1])
        # gets the weight of the end node
        self.node_distance = start_weight - end_weight # calculates the difference between the two weights
        self.move_dir = 0 # sets an unknown direction
        # 0 = not set
        # 1 = up
        # 2 = down
        # 3 = left
        # 4 = right
        if (start_node[0] == end_node[0]): # if they have the same x value, it must be a change in the y
            if (start_node[1] > end_node[1]): # if the y of the start is greater than the y of the end
                self.move_dir = 1 # moves up
            else: # otherwise
                self.move_dir = 2 # moves down
        else: # otherwise
            if (start_node[0] > end_node[0]): # if the x of the start is greater than the x of the end
                self.move_dir = 3 # moves left
            else: # otherwise
                self.move_dir = 4 # moves right
def draw_line(x1, y1, x2, y2):
    penup()
    goto(x1, y1)
    pendown()
    goto(x2, y2)
    penup()
def map_point(x1, y1, x2, y2, x_scale, y_scale, offset):
    draw_line((x1 * x_scale) + offset[0], (y1 * y_scale) + offset[1], (x2 * x_scale) + offset[0], (y2 * y_scale) + offset[1])
def is_wall(point, grid):
    if point[0] < 0 or point[1] < 0: 
        return False
    try:
        return (grid.get_point(point[0], point[1])) == 0
    except:
        return False
max_width = 127
max_height = 15
scale = 10
def simple_wall(x1, y1, x2, y2):
    global max_width
    global max_height
    global scale
    map_point(x1, y1, x2, y2, scale, scale, [-((max_width * scale)/2), -((max_height * scale)/2)])
speed(0)
delay(0)
# actual program running
print("(", max_width, "x", max_height, "):")
color("black")
width(5)
primary_grid = grid(max_width, max_height) # creates a new grid, of which the maze is a child of
primary_maze = maze(primary_grid, 1000) # creates a new maze with the parent being the grid we just created, and an upper bound of 1000
a = datetime.datetime.now();
primary_maze.generate() # generates the new maze
b = datetime.datetime.now();
print("Completed in: ", str(int((b - a).total_seconds() * 1000)), "ms") # calculates the time delta
clear()
for y in range(0, max_height):
    for x in range(0, max_width):
        if is_wall([x, y], primary_maze.parent):
            for d in range(1, 3):
                new_point = primary_maze.move_point([x, y], (2 * (d - 1)) + 1)
                if is_wall(new_point, primary_maze.parent):
                    simple_wall(x, y, new_point[0], new_point[1])
input()
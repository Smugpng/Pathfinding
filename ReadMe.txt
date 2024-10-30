How does A* pathfinding calculate and prioritize paths?
- A* pathfinding calculates each node's path cost and draws a path for the lowest cost. It achieves this by combining both positives from both Dijkstra's Algorithm and the
Greedy-Best-Search; favoring vertices close to both the starting point and goal of the grid. This puts in an equal amount of work to integrate the most efficient path.
What challenges arise when dynamically updating obstacles in real-time?
- When a new obstacle is detected within A* pathfinding, the algorithm only recalculates the affected portion of the path. Any obstacle that avoids the path isn't taken into consideration during the dynamic change, leaving the path intact. 
How could you adapt this code for larger grids or open-world settings?
- For open world settings or larger grids we would have each agent generate their own grid within a radius around them, or share grids. By doing this you can avoid drawing grids on terrain that is not used, saving memory.
What would your approach be if you were to add weighted cells (e.g., "difficult terrain" areas)?
- For weighted cell use we would have each agent calculate the sum of each path they can traverse. Depending on which path has the lowest weight the agent will decide whether to traverse that path or not.

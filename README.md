# CSCI 5611 Final Project Written Report
Nathaniel Ludwig - ludwi298
# Overview of results

I implemented an SPH fluid simulation in 2D using the Double Density Relaxation algorithm from a paper by Clavet et al. I added some additional features to the simulation, including the ability for the user to control the fluid using the mouse cursor to repel the particles and object interaction and collision. To improve the performance of the simulation, I used a spatial hash table data structure to store the particles.

## Videos
https://user-images.githubusercontent.com/37120685/208570746-15e454bb-534e-46f9-ab3e-8c67173b6f3a.mov

https://user-images.githubusercontent.com/37120685/208570770-9434fc07-ca7e-480f-8784-a4a09d317b37.mov

# Key algorithms and approaches

In an SPH simulation, the fluid is represented by a group of particles that each have their own position, velocity, and density. The density of each particle is based on the distances between it and its neighbors. A key part of the simulation is the Double Density Relaxation (DDR) step, which calculates the forces between particles. The DDR process starts by calculating the density and near density of each particle using the distances between it and its neighbors, which are efficiently queried using a spatial hash table. Then, pressure values are calculated based on the density, as well as constant rest density and stiffness values. Finally, a displacement is applied to each particle and its neighbors based on the pressure value.

https://github.com/NathanielLudwig/CSCI5611-finalproject/blob/c1af19bdba3e714d281981813832f1064dea285f/Assets/Scripts/Fluid.cs#L94-L129

To improve the performance of the simulation, a spatial hash table data structure was used to more efficiently query the neighboring particles. This data structure is organized as a grid of cells with a fixed size, where each cell contains a list of particles. When searching for neighbors, we only need to consider the particles in the cells surrounding the target particle, rather than looking at all the particles in the simulation. While the use of a spatial hash table helps to improve the efficiency of the algorithm, it is still limited by the fact that it runs on a single thread on the CPU, which limits its scalability to 10x or 100x larger simulations.

https://github.com/NathanielLudwig/CSCI5611-finalproject/blob/c1af19bdba3e714d281981813832f1064dea285f/Assets/Scripts/SpatialHashTable.cs#L27-L57

# Comparison to state of the art

One significant limitation of the algorithm I used is that it only runs on a single thread on the CPU, which can be a bottleneck in terms of performance. Modern implementations of SPH are designed to take advantage of the parallel processing capabilities of multi-core CPUs and graphics processing units (GPUs) to improve the simulation speed and the number of particles that can be simulated (https://www.gpusph.org/). These state-of-the-art SPH simulations also model complex fluid properties, such as viscosity and surface tension, to achieve more realistic results. In addition, SPH has been extended to simulate other physical phenomena, such as snow. (https://cg.informatik.uni-freiburg.de/publications/2020_SIGGRAPH_snow_v1.pdf) However, my implementation was limited to the basic functionality of the algorithm and did not include these advanced features.





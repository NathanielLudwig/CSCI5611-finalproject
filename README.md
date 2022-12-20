# CSCI 5611 Final Project Written Report
Nathaniel Ludwig - ludwi298

Full video presentation: https://drive.google.com/file/d/1Yd6AhZegCXtO207cMlTTnJWwrO64eFZP/view

Slides: https://docs.google.com/presentation/d/1_iSu5DhFxx9hNYVWkSulRMS7FaUH4erkVUwW3LBvjuM/edit?usp=sharing

Original Paper: http://www.ligum.umontreal.ca/Clavet-2005-PVFS/pvfs.pdf

# Overview of Results

I implemented a 2D smoothed particle hydrodynamics (SPH) fluid simulation using the Double Density Relaxation algorithm from a paper by Clavet et al. The rendering was done using Unity3D. In addition to the basic functionality of the algorithm, I also added some extra features to the simulation, such as allowing the user to control the fluid using the mouse cursor to repel the particles, drawing fluid with the mouse, and enabling object interaction and collision. To improve the performance of the simulation, I utilized a spatial hash table data structure to store the particles.

## Videos
https://user-images.githubusercontent.com/37120685/208570746-15e454bb-534e-46f9-ab3e-8c67173b6f3a.mov

https://user-images.githubusercontent.com/37120685/208570770-9434fc07-ca7e-480f-8784-a4a09d317b37.mov

# Key Algorithms and Approaches

In an SPH simulation, the fluid is represented by a group of particles that each have their own position, velocity, and density. The density of each particle is based on the distances between it and its neighbors. A key part of the simulation is the Double Density Relaxation (DDR) step, which calculates the forces between particles. The DDR process starts by calculating the density and near density of each particle using the distances between it and its neighbors, which are efficiently queried using a spatial hash table. Then, pressure values are calculated based on the density, as well as constant rest density and stiffness values. Finally, a displacement is applied to each particle and its neighbors based on the pressure value.

https://github.com/NathanielLudwig/CSCI5611-finalproject/blob/c1af19bdba3e714d281981813832f1064dea285f/Assets/Scripts/Fluid.cs#L94-L129

To improve the performance of the simulation, a spatial hash table data structure was used to more efficiently query the neighboring particles. This data structure is organized as a grid of cells with a fixed size, where each cell contains a list of particles. When searching for neighbors, we only need to consider the particles in the cells surrounding the target particle, rather than looking at all the particles in the simulation. While the use of a spatial hash table helps to improve the efficiency of the algorithm, it is still limited by the fact that it runs on a single thread on the CPU, which limits its scalability to 10x or 100x larger simulations.

https://github.com/NathanielLudwig/CSCI5611-finalproject/blob/c1af19bdba3e714d281981813832f1064dea285f/Assets/Scripts/SpatialHashTable.cs#L27-L57

# Comparing to State-of-the-Art Techniques

One significant limitation of the algorithm I used is that it only runs on a single thread on the CPU, which can be a bottleneck in terms of performance. Modern implementations of SPH are designed to take advantage of the parallel processing capabilities of multi-core CPUs and graphics processing units (GPUs) to improve the simulation speed and the number of particles that can be simulated (https://www.gpusph.org/). These state-of-the-art SPH simulations also model complex fluid properties, such as viscosity and surface tension, to achieve more realistic results. In addition, SPH has been extended to simulate other physical phenomena, such as snow. (https://cg.informatik.uni-freiburg.de/publications/2020_SIGGRAPH_snow_v1.pdf) However, my implementation was limited to the basic functionality of the algorithm and did not include these advanced features.

# Project Progression

## Initial Sketch
<img width="463" alt="Screenshot 2022-12-19 at 10 51 20 PM" src="https://user-images.githubusercontent.com/37120685/208586435-fa3c5690-9ee5-4b1e-ac02-e9b9b6719873.png">
The initial objective of this project was to implement the SPH algorithm and render the simulation in three dimensions. Unfortunately, the performance was not sufficient to support the number of particles required for a realistic 3D rendering. As a result, the project was limited to a 2D simulation. Additionally, although plans were made to include scenarios such as a flowing fluid or water in a cup, and to incorporate viscosity properties into the simulation, these features were not implemented due to time constraints. However, object interaction, as demonstrated in the original sketch, was included in the final simulation.

## Progress Report
<img width="372" alt="Screenshot 2022-12-19 at 10 59 01 PM" src="https://user-images.githubusercontent.com/37120685/208587409-c671e6bf-5127-42bc-acc9-07b984fcf7df.png">
Attempts to simulate the fluid in three dimensions were not successful due to performance problems, largely caused by the lack of a spatial data structure. In addition, issues with the relaxation part of the algorithm and improper parameter tuning resulted in unrealistic fluid during this stage of the project.

# Peer Feedback

During the class discussion, I received a few pieces of feedback. Two suggestions were to implement a spatial data structure and add object interaction, which were both incorporated into the final version of the simulation. Another suggestion was to include flowing water from a faucet, but this feature was not successfully implemented in the final version.

# Extension Ideas

Improving the performance of the algorithm, supporting a larger number of fluids, and rendering the simulation in 3D would significantly enhance the realism of the simulation. Additionally, improved visualization techniques, such as the use of shaders, could also contribute to the realism of the fluid. Given more time, it would also be possible to test the fluid in a variety of scenarios and to explore the properties of different types of fluids, including viscosity and elasticity, as described in the original paper.






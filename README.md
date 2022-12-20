# CSCI 5611 Final Project Written Report
Nathaniel Ludwig - ludwi298
# Overview of results

I implemented an SPH fluid simulation in 2D using the Double Density Relaxation algorithm from a paper by Clavet et al. I added some additional features to the simulation, including the ability for the user to control the fluid using the mouse cursor to repel the particles and object interaction and collision. To improve the performance of the simulation, I used a spatial hash table data structure to store the particles.

## Videos
https://user-images.githubusercontent.com/37120685/208570746-15e454bb-534e-46f9-ab3e-8c67173b6f3a.mov

https://user-images.githubusercontent.com/37120685/208570770-9434fc07-ca7e-480f-8784-a4a09d317b37.mov

# Key algorithms and approaches

In an SPH simulation, the fluid is represented by a set of particles, each of which has a position, velocity, and density. The density of each particle is determined by the distances between the particle and its neighbors. The main part of the simulation is the double density relaxation step which calculates the forces between particles. The first part of the double density relaxation is calculating the density and near density by calculating the distance between neighbors. The neighboring particles are efficiently queried using the spatial hash table. We then calculate pressure values based on the density and constant rest density and stiffness values.





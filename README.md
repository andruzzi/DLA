# DLA

A super naive usage of DLA to generate a universe as seen in EVE.
Stutters a lot, the whole thing is under-optimized and takes around 5-10mins to generate 5000+ systems but it works.

Using unity physics wasnt a good idea but it results in a few interesting variations ; for example, making walkers move too fast and having them overshoot the parent node. It sort of kills the DLA "organic structure" but results in a big difference in distance between linked nodes which adds a lot of variation between the branches.

![generation](https://user-images.githubusercontent.com/13935399/63641597-2e4fbf00-c6b1-11e9-9c79-4d1a1e0bdc81.PNG)
![result](https://user-images.githubusercontent.com/13935399/63641596-2e4fbf00-c6b1-11e9-9cba-e6e0b7d11d16.PNG)

Refs : 
* [Wikipedia](https://en.wikipedia.org/wiki/Diffusion-limited_aggregation)
* [pgc.wikidot.com](http://pcg.wikidot.com/diffusion-limited-aggregation)

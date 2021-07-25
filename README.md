# PicKinetic AR

### Algorithm flow
1. Scale and crop(into power of 2) the camera image down, for faster computation
2. Use Guassian blur to filter out noises, and dilation to emphasize lines
3. Use moore neighborhood algorithm to find contour
4. Use Cube marching algorithm to create mesh from
5. duplicate 2d mesh, and glue two plane mesh together for 3d mesh

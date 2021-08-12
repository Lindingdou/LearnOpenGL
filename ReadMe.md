# LearnOpenGL CN
learnOpenGL

# 1 简介


# 2 入门


## 2.1 


## 2.6 纹理 
### 识点总结:

- 知识点1：“增加顶点会徒增性能开销，因此采用纹理来增加对物体细节的描述”

- 知识点2：问题图像的读写可以采用SOIL库和stb_image.h库，新教程用后者

- 知识点3：除图像外，纹理也可以被用来储存大量的数据，这些数据可以发送到着色器上

- 知识点4：指定三角形的每个顶点各自对应纹理的哪个部分。这样每个顶点就会关联一个纹理坐标，用来表明该从问题图像的哪个部分采样
- 知识点5：使用纹理坐标获取纹理颜色叫做采样(Sampling)


### 示例代码：

```c++
float texCoords[] = {
    0.0f, 0.0f, // 左下角
    1.0f, 0.0f, // 右下角
    0.5f, 1.0f // 上中
};
```

### 讨论点1：纹理环绕方式
### 知识点总结：
- 知识点1：环绕方式和样式描述

|  环绕方式   | 描述  |
|  ----  | ----  |
| GL_REPEAT  | 对纹理的默认行为。重复纹理图像。 |
| GL_MIRRORED_REPEAT  | 和GL_REPEAT一样，但每次重复图片是镜像放置的。 |
|GL_CLAMP_TO_EDGE|纹理坐标会被约束在0到1之间，超出的部分会重复纹理坐标的边缘，产生一种边缘被拉伸的效果。|
|GL_CLAMP_TO_BORDER|超出的坐标为用户指定的边缘颜色。|

![](https://learnopengl-cn.github.io/img/01/06/texture_wrapping.png)

- 知识点2：glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_MIRRORED_REPEAT)函数
    - 第一个参数为：纹理目标；
    - 第二个参数为：设置的选项与应用的纹理轴；
    - 第三个参数为：环绕方式(Wrapping),r如果环绕方式指定为GL_CLAMP_TO_BORDER选项，则还需要指定一个边缘颜色。需要使用glTexParameter函数的fv后缀形式，用GL_TEXTURE_BORDER_COLOR作为它的选项，并且传递一个float数组作为边缘的颜色值，具体代码如下：
    ```c++
    float borderColor[] = { 1.0f, 1.0f, 0.0f, 1.0f };
    glTexParameterfv(GL_TEXTURE_2D, GL_TEXTURE_BORDER_COLOR, borderColor);
    ```
### 讨论点2：纹理过滤
### 知识点总结：
- 知识点1：纹理坐标不依赖于分辨率(Resolution)，它可以是任意浮点值，所以OpenGL需要知道怎样将纹理像素(Texture Pixel，也叫Texel，译注1)映射到纹理坐标。
- 知识点2：纹理过滤(Texture Filtering)的选项：GL_NEAREST和GL_LINEAR
- 知识点3：纹理坐标是给模型顶点设置的那个数组，OpenGL以这个顶点的纹理坐标数据去查找纹理图像上的像素，然后进行采样提取纹理像素的颜色。
    - GL_NEAREST（也叫邻近过滤，Nearest Neighbor Filtering）：OpenGL默认的纹理过滤方式；当设置为此种方式时，OpenGL会选择中心点最接近纹理坐标的那个像素。
   
   <div align="center">
   <img src="https://learnopengl-cn.github.io/img/01/06/filter_nearest.png"/>
   </div>
   
    - GL_LINEAR（也叫线性过滤，(Bi)linear Filtering）:基于纹理坐标附近的纹理像素，计算出一个插值，近似出这些纹理像素之间的颜色。一个纹理像素的中心距离纹理坐标越近，那么这个纹理像素的颜色对最终的样本颜色的贡献越大，是个混合色。
    <div align="center">
    <img src="https://learnopengl-cn.github.io/img/01/06/filter_linear.png"/>
    </div>
    GL_NEAREST产生了颗粒状的图案，能够清晰看到组成纹理的像素，而GL_LINEAR能够产生更平滑的图案，很难看出单个的纹理像素。
<div align="center">
<img src="https://learnopengl-cn.github.io/img/01/06/texture_filtering.png"/>
</div>
    当进行放大(Magnify)和缩小(Minify)操作的时候可以设置纹理过滤的选项，比如你可以在纹理被缩小的时候使用邻近过滤，被放大时使用线性过滤。使用glTexParameter*函数为放大和缩小指定过滤方式。
    
```c++
glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
```
### 讨论点3：多级渐远纹理
### 知识点总结：
- 知识点1：多级渐远纹理(Mipmap):距观察者的距离超过一定的阈值，OpenGL会使用不同的多级渐远纹理，即最适合物体的距离的那个;
<div align="center">
<img src="https://learnopengl-cn.github.io/img/01/06/mipmaps.png"/>
</div>

- 知识点2：glGenerateMipmaps函数用来完成一系列多级渐进纹理；

- 知识点3：在渲染中切换多级渐远纹理级别(Level)时，OpenGL在两个不同级别的多级渐远纹理层之间会产生不真实的生硬边界。就像普通的纹理过滤一样，切换多级渐远纹理级别时你也可以在两个不同多级渐远纹理级别之间使用NEAREST和LINEAR过滤。 
    
    | 过滤方式 | 描述 |
    | ---- | ---- |
    |GL_NEAREST_MIPMAP_NEAREST|	使用最邻近的多级渐远纹理来匹配像素大小，并使用邻近插值进行纹理采样|
    |GL_LINEAR_MIPMAP_NEAREST|	使用最邻近的多级渐远纹理级别，并使用线性插值进行采样|
    |GL_NEAREST_MIPMAP_LINEAR|	在两个最匹配像素大小的多级渐远纹理之间进行线性插值，使用邻近插值进行采样|
    |GL_LINEAR_MIPMAP_LINEAR|在两个邻近的多级渐远纹理之间使用线性插值，并使用线性插值进行采样|
    
    可以采用glTexParameteri函数来设置过滤方式：
    
    ```C++ 
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    ```
    <font color=red>常见错误：</font>将放大过滤的选项设置为多级渐远纹理过滤选项之一。这样没有任何效果，因为多级渐远纹理主要是使用在纹理被缩小的情况下的：纹理放大不会使用多级渐远纹理，为放大过滤设置多级渐远纹理的选项会产生一个GL_INVALID_ENUM错误代码。

### 讨论点4：加载与创建纹理
### 知识点总结：
- 知识点1：纹理图像可能被储存为各种各样的格式，每种都有自己的数据结构和排列，需要找特定的文件格式，写一个图像加载器，把图像转化为字节序列。
- 知识点2：stb_image.h是Sean Barrett的一个非常流行的单头文件图像加载库，它能够加载大部分流行的文件格式，并且能够很简单得整合到你的工程之中，这个库可以在这个网站下载 https://github.com/nothings/stb/blob/master/stb_image.h 
- 知识点3： 下载头文件，将它以stb_image.h的名字加入工程，并另创建一个新的C++文件，输入以下代码
    ```c++
    #define STB_IMAGE_IMPLEMENTATION
    #include "stb_image.h"
    ```
    通过定义STB_IMAGE_IMPLEMENTATION，预处理器会修改头文件，让其只包含相关的函数定义源码，等于是将这个头文件变为一个 .cpp 文件了。现在只需要在你的程序中包含stb_image.h并编译就可以了
- 知识点4: 使用stb_image.h加载图片，需要使用它的stbi_load函数，具体代码如下：
    ```c++
    int width, height, nrChannels;
    unsigned char *data = stbi_load("container.jpg", &width, &height, &nrChannels, 0);
    ```
    该函数首先接受一个图像文件的位置作为输入。随后需要三个int作为它的第二、第三和第四个参数，stb_image.h将会用图像的宽度、高度和颜色通道的个数填充这三个变量

### 讨论点5：生成纹理
### 知识点总结：
- 知识点1：生成纹理的流程、步骤：
  + 第1步：创建纹理的ID引用：纹理也是使用ID引用的，具体创建方式如下：
    ```c++
    unsigned int texture;
    glGenTextures(1, &texture); 
    ```
   函数中的第一个参数是需要输入生成纹理的数量，并把它们储存在第二个参数的unsigned int数组中（我们的例子中只是单独的一个unsigned int）

  + 第2步：绑定纹理：
   ```c++
   glBindTexture(GL_TEXTURE_2D, texture);
   ```
  + 第3步： 通过glTexImage2D将载入的图片数据生成纹理
   ```c++
   glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, data);
   glGenerateMipmap(GL_TEXTURE_2D);
   ```
    函数中传入的参数分别为：<font color=green>
    - 第一个参数指定了纹理目标(Target)。设置为GL_TEXTURE_2D意味着会生成与当前绑定的纹理对象在同一个目标上的纹理（任何绑定到GL_TEXTURE_1D和GL_TEXTURE_3D的纹理不会受到影响）。
    - 第二个参数为纹理指定多级渐远纹理的级别，如果你希望单独手动设置每个多级渐远纹理的级别的话。这里我们填0，也就是基本级别。
    - 第三个参数告诉OpenGL我们希望把纹理储存为何种格式。我们的图像只有RGB值，因此我们也把纹理储存为RGB值。
    - 第四个和第五个参数设置最终的纹理的宽度和高度。我们之前加载图像的时候储存了它们，所以我们使用对应的变量。
    - 下个参数应该总是被设为0（历史遗留的问题）。
    - 第七第八个参数定义了源图的格式和数据类型。我们使用RGB值加载这个图像，并把它们储存为char(byte)数组，我们将会传入对应值。
    - 最后一个参数是真正的图像数据。
    </font>

    当调用glTexImage2D时，当前绑定的纹理对象就会被附加上纹理图像。然而，目前只有基本级别(Base-level)的纹理图像被加载了，如果要使用多级渐远纹理，我们必须手动设置所有不同的图像（不断递增第二个参数）。或者，直接在生成纹理之后调用glGenerateMipmap。这会为当前绑定的纹理自动生成所有需要的多级渐远纹理。
    
    - 第4步：生成了纹理和相应的多级渐远纹理后，释放图像的内存
    ```c++
    stbi_image_free(data);
    ```
- 知识点2：整个步骤流程源码：
    ```c++
    unsigned int texture;
    glGenTextures(1, &texture);
    glBindTexture(GL_TEXTURE_2D, texture);
    // 为当前绑定的纹理对象设置环绕、过滤方式
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);   
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    // 加载并生成纹理
    int width, height, nrChannels;
    unsigned char *data = stbi_load("container.jpg", &width, &height, &nrChannels, 0);
    if (data)
    {
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, data);
        glGenerateMipmap(GL_TEXTURE_2D);
    }
    else
    {
        std::cout << "Failed to load texture" << std::endl;
    }
    stbi_image_free(data);
    ```
### 讨论点6：应用纹理
### 知识点总结：
- 知识点1：使用glDrawElements绘制图案，必须使用纹理坐标更新顶点数据：
    ```c++
    float vertices[] = {
    //    ---- 位置 ----      ---- 颜色 ----    - 纹理坐标 -
        0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,   1.0f, 1.0f,   // 右上
        0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,   1.0f, 0.0f,   // 右下
        -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,   0.0f, 0.0f,  // 左下
        -0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 0.0f,   0.0f, 1.0f   // 左上
    };
    ```
   具体的顶点格式如下：
   <div align="center">
   <img src="https://learnopengl-cn.github.io/img/01/06/vertex_attribute_pointer_interleaved_textures.png"/>
   </div>
    
    按照上面的顶点结构和内存中的顶点属性步长来设置顶点属性：

   ```c++
    glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(6 * sizeof(float)));
    glEnableVertexAttribArray(2);
   ```
- 知识点2： 调整顶点着色器使其能够接受顶点坐标为一个顶点属性，并把坐标传给片段着色器：
    ```c++
    #version 330 core
    layout (location = 0) in vec3 aPos;
    layout (location = 1) in vec3 aColor;
    layout (location = 2) in vec2 aTexCoord;

    out vec3 ourColor;
    out vec2 TexCoord;

    void main()
    {
        gl_Position = vec4(aPos, 1.0);
        ourColor = aColor;
        TexCoord = aTexCoord;
    }
    ```
    
    片段着色器应该接下来会把输出变量TexCoord作为输入变量。
- 知识点3：片段着色器也应该能访问纹理对象，GLSL有一个供纹理对象使用的内建数据类型，叫做采样器(Sampler)，它以纹理类型作为后缀。可以简单声明一个uniform sampler2D把一个纹理添加到片段着色器中，稍后我们会把纹理赋值给这个uniform。
    ```c++
    #version 330 core
    out vec4 FragColor;

    in vec3 ourColor;
    in vec2 TexCoord;

    uniform sampler2D ourTexture;

    void main()
    {
        FragColor = texture(ourTexture, TexCoord);
    }
    ```
    使用GLSL内建的texture函数来采样纹理的颜色，它第一个参数是纹理采样器，第二个参数是对应的纹理坐标。texture函数会使用之前设置的纹理参数对相应的颜色值进行采样。这个片段着色器的输出就是纹理的（插值）纹理坐标上的(过滤后的)颜色。

- 知识点4：调用glDrawElements之前绑定纹理了，它会自动把纹理赋值给片段着色器的采样器；
    ```c++
    glBindTexture(GL_TEXTURE_2D, texture);
    glBindVertexArray(VAO);
    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
    ```
    最终实现的效果：
    <div align="center">
    <img src="https://learnopengl-cn.github.io/img/01/06/textures2.png"/>
    </div>
- 知识点5：<font color=red>注意：</font>在一些驱动中，必须要对每个采样器uniform都附加上纹理单元才可以，这个会在下面介绍。
- 知识点6：可以把得到的纹理颜色与顶点颜色混合，来获得更有趣的效果。
    ```c++
    FragColor = texture(ourTexture, TexCoord) * vec4(ourColor, 1.0);
    ```
    最终实现的效果：
    <div align="center">
    <img src="https://learnopengl-cn.github.io/img/01/06/textures_funky.png"/>
    </div>

### 讨论点7：纹理单元
### 知识点总结：
- 知识点1：纹理单元(Texture Unit)：使用glUniform1i，可以给纹理采样器分配一个位置值，能够在一个片段着色器中设置多个纹理。
    - 一个纹理的默认纹理单元是0，它是默认的激活纹理单元.
    - 纹理单元的主要目的是让着色器中可以使用多于一个的纹理;
    - 通过把纹理单元赋值给采样器，可以一次绑定多个纹理，只要激活对应的纹理单元,就可以像glBindTexture一样，可以使用glActiveTexture激活纹理单元，传入需要使用的纹理单元。
    ```c++
    glActiveTexture(GL_TEXTURE0); // 在绑定纹理之前先激活纹理单元
    glBindTexture(GL_TEXTURE_2D, texture);
    ```
    - 激活纹理单元之后，接下来的glBindTexture函数调用会绑定这个纹理到当前激活的纹理单元，纹理单元GL_TEXTURE0默认总是被激活，所以当使用glBindTexture的时候，无需激活任何纹理单元。
- 知识点2：OpenGL至少保证有16个纹理单元供你使用，也就是说你可以激活从GL_TEXTURE0到GL_TEXTRUE15。
    - 它们都是按顺序定义的，所以也可以通过GL_TEXTURE0 + 8的方式获得GL_TEXTURE8，这在当需要循环一些纹理单元的时候会很有用。
    ```c++
    #version 330 core
    ...

    uniform sampler2D texture1;
    uniform sampler2D texture2;

    void main()
    {
        FragColor = mix(texture(texture1, TexCoord), texture(texture2, TexCoord), 0.2);
    }
    ```
    - 最终输出颜色现在是两个纹理的结合。GLSL内建的mix函数需要接受两个值作为参数，并对它们根据第三个参数进行线性插值。如果第三个值是0.0，它会返回第一个输入；如果是1.0，会返回第二个输入值。0.2会返回80%的第一个输入颜色和20%的第二个输入颜色，即返回两个纹理的混合色。
    - 先绑定两个纹理到对应的纹理单元，然后定义哪个uniform采样器对应哪个纹理单元。
        ```c++
        glActiveTexture(GL_TEXTURE0);
        glBindTexture(GL_TEXTURE_2D, texture1);
        glActiveTexture(GL_TEXTURE1);
        glBindTexture(GL_TEXTURE_2D, texture2);

        glBindVertexArray(VAO);
        glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
        ```
    - 还要通过使用glUniform1i设置每个采样器的方式告诉OpenGL每个着色器采样器属于哪个纹理单元
        ```c++
        ourShader.use(); // 不要忘记在设置uniform变量之前激活着色器程序！
        glUniform1i(glGetUniformLocation(ourShader.ID, "texture1"), 0); // 手动设置
        ourShader.setInt("texture2", 1); // 或者使用着色器类设置

        while(...) 
        {
            [...]
        }
        ```
        最终实现的效果：
    <div align="center">
    <img src="https://learnopengl-cn.github.io/img/01/06/textures_combined.png"/>
    </div>

    - 因为OpenGL要求y轴0.0坐标是在图片的底部的，但是图片的y轴0.0坐标通常在顶部。很幸运，stb_image.h能够在图像加载时帮助我们翻转y轴，只需要在加载任何图像前加入以下语句即可.
        ```c++
        stbi_set_flip_vertically_on_load(true);
        ```
    <div align="center">
    <img src="https://learnopengl-cn.github.io/img/01/06/textures_combined2.png"/>
    </div>


## 2.7 变换
### 讨论点1：向量
### 知识点总结：
- 知识点1：点乘：两个向量的点乘等于它们的数乘结果乘以两个向量之间夹角的余弦值
<div align=center>
<img src="https://latex.vimsky.com/test.image.latex.php?fmt=svg&val=%255Cdpi%257B150%257D%2520%255Clarge%2520%255Cbar%257Bv%257D%2520%255Ccdot%2520%255Cbar%257Bk%257D%2520%253D%2520%257C%257C%255Cbar%257Bv%257D%257C%257C%2520%255Ccdot%2520%257C%257C%255Cbar%257Bk%257D%257C%257C%2520%255Ccdot%2520%255Ccos%2520%255Ctheta&dl=0" />
</div>
- 知识点2：也可以通过点乘的结果计算两个非单位向量的夹角，点乘的结果除以两个向量的长度之积，得到的结果就是夹角的余弦值，即cosθ.
<div align=center>
<img src="https://latex.vimsky.com/test.image.latex.php?fmt=svg&val=%255Cdpi%257B150%257D%2520%255Clarge%2520%255Ccos%2520%255Ctheta%2520%253D%2520%255Cfrac%257B%255Cbar%257Bv%257D%2520%255Ccdot%2520%255Cbar%257Bk%257D%257D%257B%257C%257C%255Cbar%257Bv%257D%257C%257C%2520%255Ccdot%2520%257C%257C%255Cbar%257Bk%257D%257C%257C%257D&dl=0"/>
</div>
- 知识点3：点乘运算方式：
<div align=center>
<img src="https://latex.vimsky.com/test.image.latex.php?fmt=svg&val=%255Cdpi%257B150%257D%2520%255Clarge%2520%255Cbegin%257Bpmatrix%257D%2520%255Ccolor%257Bred%257D%257B0.6%257D%2520%255C%255C%2520-%255Ccolor%257Bgreen%257D%257B0.8%257D%2520%255C%255C%2520%255Ccolor%257Bblue%257D0%2520%255Cend%257Bpmatrix%257D%2520%255Ccdot%2520%255Cbegin%257Bpmatrix%257D%2520%255Ccolor%257Bred%257D0%2520%255C%255C%2520%255Ccolor%257Bgreen%257D1%2520%255C%255C%2520%255Ccolor%257Bblue%257D0%2520%255Cend%257Bpmatrix%257D%2520%253D%2520%2528%255Ccolor%257Bred%257D%257B0.6%257D%2520*%2520%255Ccolor%257Bred%257D0%2529%2520%26plus%3B%2520%2528-%255Ccolor%257Bgreen%257D%257B0.8%257D%2520*%2520%255Ccolor%257Bgreen%257D1%2529%2520%26plus%3B%2520%2528%255Ccolor%257Bblue%257D0%2520*%2520%255Ccolor%257Bblue%257D0%2529%2520%253D%2520-0.8&dl=0" width=500/>
</div>

- 知识点4：叉乘只在3D空间中有定义，它需要两个不平行向量作为输入，生成一个正交于两个输入向量的第三个向量。如果输入的两个向量也是正交的，那么叉乘之后将会产生3个互相正交的向量。
<div align=center>
<img src="https://learnopengl-cn.github.io/img/01/07/vectors_crossproduct.png"/>
</div>

- 知识点5：叉乘运算方式：
<div align=center>
<img src="https://latex.vimsky.com/test.image.latex.php?fmt=svg&val=%255Cdpi%257B150%257D%2520%255Clarge%2520%255Cbegin%257Bpmatrix%257D%2520%255Ccolor%257Bred%257D%257BA_%257Bx%257D%257D%2520%255C%255C%2520%255Ccolor%257Bgreen%257D%257BA_%257By%257D%257D%2520%255C%255C%2520%255Ccolor%257Bblue%257D%257BA_%257Bz%257D%257D%2520%255Cend%257Bpmatrix%257D%2520%255Ctimes%2520%255Cbegin%257Bpmatrix%257D%2520%255Ccolor%257Bred%257D%257BB_%257Bx%257D%257D%2520%255C%255C%2520%255Ccolor%257Bgreen%257D%257BB_%257By%257D%257D%2520%255C%255C%2520%255Ccolor%257Bblue%257D%257BB_%257Bz%257D%257D%2520%255Cend%257Bpmatrix%257D%2520%253D%2520%255Cbegin%257Bpmatrix%257D%2520%255Ccolor%257Bgreen%257D%257BA_%257By%257D%257D%2520%255Ccdot%2520%255Ccolor%257Bblue%257D%257BB_%257Bz%257D%257D%2520-%2520%255Ccolor%257Bblue%257D%257BA_%257Bz%257D%257D%2520%255Ccdot%2520%255Ccolor%257Bgreen%257D%257BB_%257By%257D%257D%2520%255C%255C%2520%255Ccolor%257Bblue%257D%257BA_%257Bz%257D%257D%2520%255Ccdot%2520%255Ccolor%257Bred%257D%257BB_%257Bx%257D%257D%2520-%2520%255Ccolor%257Bred%257D%257BA_%257Bx%257D%257D%2520%255Ccdot%2520%255Ccolor%257Bblue%257D%257BB_%257Bz%257D%257D%2520%255C%255C%2520%255Ccolor%257Bred%257D%257BA_%257Bx%257D%257D%2520%255Ccdot%2520%255Ccolor%257Bgreen%257D%257BB_%257By%257D%257D%2520-%2520%255Ccolor%257Bgreen%257D%257BA_%257By%257D%257D%2520%255Ccdot%2520%255Ccolor%257Bred%257D%257BB_%257Bx%257D%257D%2520%255Cend%257Bpmatrix%257D&dl=0" width=500/>
</div>

- 知识点6：矩阵的加法和减法只对同维度的矩阵才是有定义。
- 知识点7：简单来说，标量数乘标量就是用它的值缩放(Scale)矩阵的所有元素。
### 讨论点2：矩阵
- 知识点8：矩阵乘法的限制：
    - 只有当左侧矩阵的列数与右侧矩阵的行数相等，两个矩阵才能相乘。
    - 矩阵相乘不遵守交换律(Commutative)，也就是说A⋅B≠B⋅A。
    <div align=center>
    <img src="https://learnopengl-cn.github.io/img/01/07/matrix_multiplication.png"/>
    </div>
### 讨论点3：矩阵与向量相乘
- 知识点9：缩放：对一个向量进行缩放(Scaling)就是对向量的长度进行缩放，而保持它的方向不变。
- 知识点10：不均匀(Non-uniform)缩放/均匀缩放(Uniform Scale)：每个轴的缩放因子是否一样
- 知识点11：缩放变量表示为(S1,S2,S3)，则可以为任意向量(x,y,z)定义一个缩放矩阵
<div align=center>
    <img src="https://latex.vimsky.com/test.image.latex.php?fmt=svg&val=%255Cdpi%257B150%257D%2520%255Clarge%2520%255Cbegin%257Bbmatrix%257D%2520%255Ccolor%257Bred%257D%257BS_1%257D%2520%2526%2520%255Ccolor%257Bred%257D0%2520%2526%2520%255Ccolor%257Bred%257D0%2520%2526%2520%255Ccolor%257Bred%257D0%2520%255C%255C%2520%255Ccolor%257Bgreen%257D0%2520%2526%2520%255Ccolor%257Bgreen%257D%257BS_2%257D%2520%2526%2520%255Ccolor%257Bgreen%257D0%2520%2526%2520%255Ccolor%257Bgreen%257D0%2520%255C%255C%2520%255Ccolor%257Bblue%257D0%2520%2526%2520%255Ccolor%257Bblue%257D0%2520%2526%2520%255Ccolor%257Bblue%257D%257BS_3%257D%2520%2526%2520%255Ccolor%257Bblue%257D0%2520%255C%255C%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D1%2520%255Cend%257Bbmatrix%257D%2520%255Ccdot%2520%255Cbegin%257Bpmatrix%257D%2520x%2520%255C%255C%2520y%2520%255C%255C%2520z%2520%255C%255C%25201%2520%255Cend%257Bpmatrix%257D%2520%253D%2520%255Cbegin%257Bpmatrix%257D%2520%255Ccolor%257Bred%257D%257BS_1%257D%2520%255Ccdot%2520x%2520%255C%255C%2520%255Ccolor%257Bgreen%257D%257BS_2%257D%2520%255Ccdot%2520y%2520%255C%255C%2520%255Ccolor%257Bblue%257D%257BS_3%257D%2520%255Ccdot%2520z%2520%255C%255C%25201%2520%255Cend%257Bpmatrix%257D&dl=0" width=400/>
    </div>

- 知识点12：位移(Translation)是在原始向量的基础上加上另一个向量从而获得一个在不同位置的新向量的过程，从而在位移向量基础上移动了原始向量。
- 知识点13：如果把位移向量表示为(Tx,Ty,Tz)，就能把位移矩阵定义为：
<div align=center>
    <img src="https://latex.vimsky.com/test.image.latex.php?fmt=svg&val=%255Cdpi%257B150%257D%2520%255Clarge%2520%255Cbegin%257Bbmatrix%257D%2520%255Ccolor%257Bred%257D1%2520%2526%2520%255Ccolor%257Bred%257D0%2520%2526%2520%255Ccolor%257Bred%257D0%2520%2526%2520%255Ccolor%257Bred%257D%257BT_x%257D%2520%255C%255C%2520%255Ccolor%257Bgreen%257D0%2520%2526%2520%255Ccolor%257Bgreen%257D1%2520%2526%2520%255Ccolor%257Bgreen%257D0%2520%2526%2520%255Ccolor%257Bgreen%257D%257BT_y%257D%2520%255C%255C%2520%255Ccolor%257Bblue%257D0%2520%2526%2520%255Ccolor%257Bblue%257D0%2520%2526%2520%255Ccolor%257Bblue%257D1%2520%2526%2520%255Ccolor%257Bblue%257D%257BT_z%257D%2520%255C%255C%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D1%2520%255Cend%257Bbmatrix%257D%2520%255Ccdot%2520%255Cbegin%257Bpmatrix%257D%2520x%2520%255C%255C%2520y%2520%255C%255C%2520z%2520%255C%255C%25201%2520%255Cend%257Bpmatrix%257D%2520%253D%2520%255Cbegin%257Bpmatrix%257D%2520x%2520%26plus%3B%2520%255Ccolor%257Bred%257D%257BT_x%257D%2520%255C%255C%2520y%2520%26plus%3B%2520%255Ccolor%257Bgreen%257D%257BT_y%257D%2520%255C%255C%2520z%2520%26plus%3B%2520%255Ccolor%257Bblue%257D%257BT_z%257D%2520%255C%255C%25201%2520%255Cend%257Bpmatrix%257D&dl=0" width=400/>
    </div>

- 知识点14：旋转矩阵在3D空间中每个单位轴都有不同定义，旋转角度用θ表示：
    
    - 沿x轴旋转：
    <div align=center>
    <img src="https://latex.vimsky.com/test.image.latex.php?fmt=svg&val=%255Cdpi%257B150%257D%2520%255Clarge%2520%255Cbegin%257Bbmatrix%257D%2520%255Ccolor%257Bred%257D1%2520%2526%2520%255Ccolor%257Bred%257D0%2520%2526%2520%255Ccolor%257Bred%257D0%2520%2526%2520%255Ccolor%257Bred%257D0%2520%255C%255C%2520%255Ccolor%257Bgreen%257D0%2520%2526%2520%255Ccolor%257Bgreen%257D%257B%255Ccos%2520%255Ctheta%257D%2520%2526%2520-%2520%255Ccolor%257Bgreen%257D%257B%255Csin%2520%255Ctheta%257D%2520%2526%2520%255Ccolor%257Bgreen%257D0%2520%255C%255C%2520%255Ccolor%257Bblue%257D0%2520%2526%2520%255Ccolor%257Bblue%257D%257B%255Csin%2520%255Ctheta%257D%2520%2526%2520%255Ccolor%257Bblue%257D%257B%255Ccos%2520%255Ctheta%257D%2520%2526%2520%255Ccolor%257Bblue%257D0%2520%255C%255C%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D1%2520%255Cend%257Bbmatrix%257D%2520%255Ccdot%2520%255Cbegin%257Bpmatrix%257D%2520x%2520%255C%255C%2520y%2520%255C%255C%2520z%2520%255C%255C%25201%2520%255Cend%257Bpmatrix%257D%2520%253D%2520%255Cbegin%257Bpmatrix%257D%2520x%2520%255C%255C%2520%255Ccolor%257Bgreen%257D%257B%255Ccos%2520%255Ctheta%257D%2520%255Ccdot%2520y%2520-%2520%255Ccolor%257Bgreen%257D%257B%255Csin%2520%255Ctheta%257D%2520%255Ccdot%2520z%2520%255C%255C%2520%255Ccolor%257Bblue%257D%257B%255Csin%2520%255Ctheta%257D%2520%255Ccdot%2520y%2520%26plus%3B%2520%255Ccolor%257Bblue%257D%257B%255Ccos%2520%255Ctheta%257D%2520%255Ccdot%2520z%2520%255C%255C%25201%2520%255Cend%257Bpmatrix%257D&dl=0" width=450/>
    </div>
   
    - 沿y轴旋转：
    <div align=center>
    <img src="https://latex.vimsky.com/test.image.latex.php?fmt=svg&val=%255Cdpi%257B150%257D%2520%255Clarge%2520%255Cbegin%257Bbmatrix%257D%2520%255Ccolor%257Bred%257D%257B%255Ccos%2520%255Ctheta%257D%2520%2526%2520%255Ccolor%257Bred%257D0%2520%2526%2520%255Ccolor%257Bred%257D%257B%255Csin%2520%255Ctheta%257D%2520%2526%2520%255Ccolor%257Bred%257D0%2520%255C%255C%2520%255Ccolor%257Bgreen%257D0%2520%2526%2520%255Ccolor%257Bgreen%257D1%2520%2526%2520%255Ccolor%257Bgreen%257D0%2520%2526%2520%255Ccolor%257Bgreen%257D0%2520%255C%255C%2520-%2520%255Ccolor%257Bblue%257D%257B%255Csin%2520%255Ctheta%257D%2520%2526%2520%255Ccolor%257Bblue%257D0%2520%2526%2520%255Ccolor%257Bblue%257D%257B%255Ccos%2520%255Ctheta%257D%2520%2526%2520%255Ccolor%257Bblue%257D0%2520%255C%255C%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D1%2520%255Cend%257Bbmatrix%257D%2520%255Ccdot%2520%255Cbegin%257Bpmatrix%257D%2520x%2520%255C%255C%2520y%2520%255C%255C%2520z%2520%255C%255C%25201%2520%255Cend%257Bpmatrix%257D%2520%253D%2520%255Cbegin%257Bpmatrix%257D%2520%255Ccolor%257Bred%257D%257B%255Ccos%2520%255Ctheta%257D%2520%255Ccdot%2520x%2520%26plus%3B%2520%255Ccolor%257Bred%257D%257B%255Csin%2520%255Ctheta%257D%2520%255Ccdot%2520z%2520%255C%255C%2520y%2520%255C%255C%2520-%2520%255Ccolor%257Bblue%257D%257B%255Csin%2520%255Ctheta%257D%2520%255Ccdot%2520x%2520%26plus%3B%2520%255Ccolor%257Bblue%257D%257B%255Ccos%2520%255Ctheta%257D%2520%255Ccdot%2520z%2520%255C%255C%25201%2520%255Cend%257Bpmatrix%257D&dl=0" width=450/>
    </div>
   
    - 沿z轴旋转：
    <div align=center>
    <img src="https://latex.vimsky.com/test.image.latex.php?fmt=svg&val=%255Cdpi%257B150%257D%2520%255Clarge%2520%255Cbegin%257Bbmatrix%257D%2520%255Ccolor%257Bred%257D%257B%255Ccos%2520%255Ctheta%257D%2520%2526%2520-%2520%255Ccolor%257Bred%257D%257B%255Csin%2520%255Ctheta%257D%2520%2526%2520%255Ccolor%257Bred%257D0%2520%2526%2520%255Ccolor%257Bred%257D0%2520%255C%255C%2520%255Ccolor%257Bgreen%257D%257B%255Csin%2520%255Ctheta%257D%2520%2526%2520%255Ccolor%257Bgreen%257D%257B%255Ccos%2520%255Ctheta%257D%2520%2526%2520%255Ccolor%257Bgreen%257D0%2520%2526%2520%255Ccolor%257Bgreen%257D0%2520%255C%255C%2520%255Ccolor%257Bblue%257D0%2520%2526%2520%255Ccolor%257Bblue%257D0%2520%2526%2520%255Ccolor%257Bblue%257D1%2520%2526%2520%255Ccolor%257Bblue%257D0%2520%255C%255C%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D1%2520%255Cend%257Bbmatrix%257D%2520%255Ccdot%2520%255Cbegin%257Bpmatrix%257D%2520x%2520%255C%255C%2520y%2520%255C%255C%2520z%2520%255C%255C%25201%2520%255Cend%257Bpmatrix%257D%2520%253D%2520%255Cbegin%257Bpmatrix%257D%2520%255Ccolor%257Bred%257D%257B%255Ccos%2520%255Ctheta%257D%2520%255Ccdot%2520x%2520-%2520%255Ccolor%257Bred%257D%257B%255Csin%2520%255Ctheta%257D%2520%255Ccdot%2520y%2520%255C%255C%2520%255Ccolor%257Bgreen%257D%257B%255Csin%2520%255Ctheta%257D%2520%255Ccdot%2520x%2520%26plus%3B%2520%255Ccolor%257Bgreen%257D%257B%255Ccos%2520%255Ctheta%257D%2520%255Ccdot%2520y%2520%255C%255C%2520z%2520%255C%255C%25201%2520%255Cend%257Bpmatrix%257D&dl=0" width=450/>
    </div>
   
### 讨论点4：矩阵的组合
- 知识点15：
<div align=center>
    <img src="https://latex.vimsky.com/test.image.latex.php?fmt=svg&val=%255Cdpi%257B150%257D%2520%255Clarge%2520%255Cbegin%257Bbmatrix%257D%2520%255Ccolor%257Bred%257D2%2520%2526%2520%255Ccolor%257Bred%257D0%2520%2526%2520%255Ccolor%257Bred%257D0%2520%2526%2520%255Ccolor%257Bred%257D1%2520%255C%255C%2520%255Ccolor%257Bgreen%257D0%2520%2526%2520%255Ccolor%257Bgreen%257D2%2520%2526%2520%255Ccolor%257Bgreen%257D0%2520%2526%2520%255Ccolor%257Bgreen%257D2%2520%255C%255C%2520%255Ccolor%257Bblue%257D0%2520%2526%2520%255Ccolor%257Bblue%257D0%2520%2526%2520%255Ccolor%257Bblue%257D2%2520%2526%2520%255Ccolor%257Bblue%257D3%2520%255C%255C%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D0%2520%2526%2520%255Ccolor%257Bpurple%257D1%2520%255Cend%257Bbmatrix%257D%2520.%2520%255Cbegin%257Bbmatrix%257D%2520x%2520%255C%255C%2520y%2520%255C%255C%2520z%2520%255C%255C%25201%2520%255Cend%257Bbmatrix%257D%2520%253D%2520%255Cbegin%257Bbmatrix%257D%2520%255Ccolor%257Bred%257D2x%2520%26plus%3B%2520%255Ccolor%257Bred%257D1%2520%255C%255C%2520%255Ccolor%257Bgreen%257D2y%2520%26plus%3B%2520%255Ccolor%257Bgreen%257D2%2520%255C%255C%2520%255Ccolor%257Bblue%257D2z%2520%26plus%3B%2520%255Ccolor%257Bblue%257D3%2520%255C%255C%25201%2520%255Cend%257Bbmatrix%257D&dl=0" width=450/>
    </div>

### 讨论点5：GLM
- 知识点16：GLM是OpenGL Mathematics的缩写，它是一个只有头文件的库，也就是说我们只需包含对应的头文件就行了，不用链接和编译。
- 知识点17：GLM库从0.9.9版本起，默认会将矩阵类型初始化为一个零矩阵（所有元素均为0），而不是单位矩阵（对角元素为1，其它元素为0）。如果你使用的是0.9.9或0.9.9以上的版本，你需要将所有的矩阵初始化改为 glm::mat4 mat = glm::mat4(1.0f)。
- 知识点18：使用GLM库的步骤：
    - 引用GLM库：
    ```c++
    #include <glm/glm.hpp>
    #include <glm/gtc/matrix_transform.hpp>
    #include <glm/gtc/type_ptr.hpp>
    ```
    - 变换操作(平移）：
    ```c++
    glm::vec4 vec(1.0f, 0.0f, 0.0f, 1.0f);
    // 译注：下面就是矩阵初始化的一个例子，如果使用的是0.9.9及以上版本
    // 下面这行代码就需要改为:
    // glm::mat4 trans = glm::mat4(1.0f)
    // 之后将不再进行提示
    glm::mat4 trans;
    trans = glm::translate(trans, glm::vec3(1.0f, 1.0f, 0.0f));
    vec = trans * vec;
    std::cout << vec.x << vec.y << vec.z << std::endl;
    ```
    - 变换操作（旋转、缩放）：
    ```c++
    glm::mat4 trans;
    trans = glm::rotate(trans, glm::radians(90.0f), glm::vec3(0.0, 0.0, 1.0));
    trans = glm::scale(trans, glm::vec3(0.5, 0.5, 0.5));
    ```
    - 将矩阵传递给着色器：
    ```c++
    #version 330 core
    layout (location = 0) in vec3 aPos;
    layout (location = 1) in vec2 aTexCoord;

    out vec2 TexCoord;

    uniform mat4 transform;

    void main()
    {
        gl_Position = transform * vec4(aPos, 1.0f);
        TexCoord = vec2(aTexCoord.x, 1.0 - aTexCoord.y);
    }
    ```
    - 查询uniform变量地址，把矩阵数据发送给着色器
    ```c++
    unsigned int transformLoc = glGetUniformLocation(ourShader.ID, "transform");
    glUniformMatrix4fv(transformLoc, 1, GL_FALSE, glm::value_ptr(trans));
    ```
    <font Color=greenblue>glUniformMatrix4fv函数的四个参数解释</font>：第一个参数是uniform的位置值。第二个参数表示将要发送多少个矩阵，这里是1。第三个参数询问是否希望对我们的矩阵进行置换(Transpose)，也就是说交换我们矩阵的行和列。OpenGL开发者通常使用一种内部矩阵布局，叫做**列主序(Column-major Ordering)布局**。GLM的默认布局就是列主序，所以并不需要置换矩阵，填GL_FALSE。最后一个参数是真正的矩阵数据，**但是GLM并不是把它们的矩阵储存为OpenGL所希望接受的那种**，因此要先用GLM的自带的函数value_ptr来变换这些数据。
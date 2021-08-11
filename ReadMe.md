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
- 知识点1：纹理坐标不依赖于分辨率(Resolution)，它可以是任意浮点值，所以OpenGL需要知道怎样将纹理像素(Texture Pixel，也叫Texel，译注1)映射到纹理坐标
  


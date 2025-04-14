using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotter : MonoBehaviour
{
    // 捕获主摄像机渲染的内容，返回一个缩小后的截图
    public Texture2D CaptureScreenShot()
    {
        // 1. 获取屏幕尺寸
        int width = Screen.width;
        int height = Screen.height;

        /* 2. 创建渲染纹理
        创建一个临时的渲染纹理 rt （摄像机的画布）
        24: 深度缓存区的位数 */
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);

        // 3. 检查主摄像机是否存在
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError(Constants.CAMERA_NOT_FOUND);
            return null;
        }

        // 4. 设置摄像机的渲染目标
        mainCamera.targetTexture = rt;
        RenderTexture.active = rt;
        mainCamera.Render();

        // 5. 读取像素数据
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false); // 创建 Texture2D 对象存储屏幕内容
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0); // 使用 ReadPixels 从 RenderTexture 中读取像素到 Texture2D
        screenShot.Apply(); // 应用更改，将数据写入内存

        // 6.重置渲染目标并释放资源
        mainCamera.targetTexture = null; // 将主摄像机的渲染目标重置为默认值
        RenderTexture.active = null; // 清除当前激活的渲染纹理
        RenderTexture.ReleaseTemporary(rt); // 释放临时渲染纹理，避免内存泄漏

        // 7. 缩小截图
        Texture2D resizedScreenShot = ResizeTexture(screenShot, width / 6, height / 6);

        // 8. 销毁原始截图，释放内存
        Destroy(screenShot);

        return resizedScreenShot;
    }

    // 将输入的 Texture2D 缩小到指定的分辨率
    private Texture2D ResizeTexture(Texture2D original, int newWidth, int newHeight)
    {
        // 1. 创建渲染纹理：创建一个目标分辨率相匹配的渲染纹理，并激活
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight, 24);
        RenderTexture.active = rt;

        /* 2. 使用 GPU 缩放
            使用 GPU 的 Graphic.Blit 将 original 的像素数据拷贝并缩放到 rt 
            使用 GPU 操作比手动逐个像素缩放效率更高，适合实时操作 */
        Graphics.Blit(original, rt);

        // 3. 读取缩放后的数据
        Texture2D resized = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
        resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resized.Apply();

        // 4. 释放资源
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return resized;
    }
}

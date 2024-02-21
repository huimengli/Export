### 这里保存的是过去的版本

### Author: [huimengli](https://github.com/huimengli)
___

因为unity编译过程中不可以调用UnityEditor.dll
因此我将编译出的dll分为了两个版本,分别供给开发环境和编译环境

___

我想了一个别的方法,增加了命名空间:Export.Attribute.Editor
在代码中使用:
```
#if UNITY_EDITOR

	using Export.Attribute.Editor

#endif
```
这种方式处理不知道行不行
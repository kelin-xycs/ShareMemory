# ShareMemory
一个用 C# 实现的 No Sql 数据库 ， 也可以说是 分布式 缓存 ， 用于作为 集群 的 共享内存





ShareMemory 是 一个用 C# 实现的 No Sql 数据库 ， 也可以说是 分布式 缓存 ， 用于作为 集群 的 共享内存 。 

构建 集群 的 关键是 共享内存 。 ShareMemory 可以作为 集群 的 共享内存 ， 帮助 构建 集群 ， 这是 ShareMemory 的 第一 设计目标 。

ShareMemory 支持 2 种 数据结构 ： 字典（Dictionary）  队列（Queue） 。

支持 6 大类 数据类型 ： Value Type ， string ， Simple Object ， Walue Type 数组 ， string 数组 ， Simple Object 数组 。

可以将这 6 大类 数据类型 存放到 ShareMemory 。

Simple Object 是指 属性（Property）和 字段（Field） 类型 是 Value Type ， string 的 对象 ， 简单的说 ， 不支持 对象嵌套 。 这部分会在下面 序列化 的 部分详细介绍 。

ShareMemory 提供的 数据操作 都是 原子操作 ， 是 线程安全 的 。

解决方案 中 包含 6 个 项目：

Client ： 用于 Demo 的 Client
Server ： 用于 Demo 的 Server
ShareMemory ： ShareMemory 核心库 ， 用于 Server 端
ShareMemory.Client ： ShareMemory 客户端库 ， 用于 Client 端
ShareMemory.Serialization ： ShareMemory 序列化库 ， 用于 序列化
Test ： 用于测试 ShareMemory.Serialization 的 测试项目


ShareMemory 服务器端 在 App.config 中配置 字典 和 队列，在 AppSettings 中 通过 “ShareMemory.Dics” 和 “ShareMemory.Queues” 2 个 key 来配置 字典 和 队列 ， 如 add key="ShareMemory.Dics" value="Dic1, Dic2, Dic3"  add key="ShareMemory.Queues" value="Queue1, Queue2, Queue3" ， Dic1 Dic2 Dic3 表示要创建的 字典 ， Queue1 Queue2 Queue3 表示要创建的 队列 ， 字典名 队列名之间用 逗号 “,” 隔开 。 这样 ShareMemory Host 在启动时会创建 Dic1 Dic2 Dic3 3 个 字典 ， 和 Queue1 Queue2 Queue3 3 个 队列 。


ShareMemory 客户端 通过 ShareMemory.Client 库 提供的 Helper 类 ， Dic 类 ， Q 类 来 访问 ShareMemory 服务器端 。

Helper类提供 GetDic() 方法 ， 返回 Dic 对象 。 和 GetQ() 方法，返回 Q 对象 。

Dic 提供 Set(key, value) 方法 ， Get<T>(key) 方法 ， TryGet<T>(key out value) 方法 ， Remove(key) 方法 。 Set() 方法 新增键值对 或者 修改键值对的值 ， 如果 键值对 不存在，则新增键值对，如果键值对已存在，则更新值 。 Get() 方法从 Dic 取得值 ， 对于 引用类型 ， 如果 键值对 不存在 ， 则返回 null 。 TryGet() 方法也是从 Dic 取得值，通过 out value 参数返回， 若 键值对 不存在，则 Get() 方法返回值为 false 。 TryGet<T>() 方法是对 Value Type 设计的 ， 因为 Value Type 不能根据返回值为 null 来判断键值对在 Dic 中是否存在 。 Remove() 方法 移除 键值对 ， 如果 键值对 不存在 ， 也不会报错 。 


Q 提供 En() 方法 ， De<T> ， TryDe<T>(out value) 方法 。 En() 方法将 对象 放入 队列 ， De() 方法从 队列 取出对象 ， 对于 引用类型 ， 若返回 null ， 表示 队列 为空 。 TryDe() 方法也是从 队列 取出 对象 ， 通过 out value 参数返回 ， 若 队列 为空 ， TryDe() 方法返回值为 false 。 TryDe() 方法是对 Value Type 设计的 ， 因为 Value Type 不能根据返回值为 null 来判断 队列 是否为空 。 
  

接下来 说明 一下 序列化 的 格式 ：

序列化由 ShareMemory.Serialization 项目完成 ， 序列化格式 是 这样的 ：

比如 ， 有一个 Simple Object ， 包含有 1 个 int A 属性 ， 1 个 string B 字段 ， A = 2 , B = "Hello" ， 那么 ， 序列化产生这样一个字符串 ： 

“o 1 A1 21 B5 Hello”

把 这个 字符串 通过 Encoding.Utf8 转成 byte 数组 ， 就是 序列化 的 结果 了 。   

这个 字符串 的 开头 是 “o” ， 这表示 Simple Object 对象 ， 后面跟着 一个 空格 ， 空格 后面 的 “1” 表示 接下来 的 数据长度 是 1 个 字符 。 这个数据就是 后面的 “A” ， 这表示 A 属性的 属性名 ， “A” 后面 有一个 “1” ， 这表示 下一项 的 数据长度 是 1 ， 这个数据就是 后面的 “2” ， 这是 A 属性的 值 ， 以此类推 ， “2” 后面 紧跟着 的 “1” 是 下一项 的 数据长度 ， 这个数据就是 “B” 字符 ， 这表示 B 属性的 属性名 ， “B” 后面的 “5” 表示 下一项 的 数据长度 ， 这个数据就是 “Hello” 。 这样就完成了 对 这个 Simple Object 的 序列化 。   


大家可以看到 ， 对于 int 类型 ， 序列化 的 方式 是 ToString() ， 对于 string ， 就是 string 本身 ， 实际上 ， 目前 除了 DateTime 外 ， 其它的 Value Type 都是 以 ToString() 的方式来 序列化 ， DateTime 是 取 Ticks 属性 ， 当然 string 就是 string 本身 。    

如果是 单独 序列化 一个 Value Type 的值 ， 比如 int a = 2; 那么 就是 “1 2” 这样一个 字符串 ， 如上所述 ， “1” 表示 数据长度 ， “2” 表示 数据值 。    

对于 数组类型 ， 举个例子 ， 假设 有一个 数组 ， 放了 2 个 Simple Object 对象 ， 这个 Simple Object 对象 就是 上面说的 那个 ， 那 序列化 后的 字符串 是 这样的 ：   

“a 2 18 o 1 A1 21 B5 Hello18 o 1 A1 21 B5 Hello”

第一个字符 “a” 表示 数组 ， 这是 固定的 ， 后面跟一个 空格 ， 这也是固定的 。 空格后面是 “2” ， 表示 数组长度 ， 即 数组元素 的 个数 。 “2” 后面跟一个 空格 ， 这也是固定的 ， 空格后面是 “18” ， 表示 接下来 的 元素 的 长度 ， “18” 后面跟一个 空格 ， 这也是固定的 。 空格之后 就是 元素 的 内容 。 这个内容 ， 就是上面我们讲过的 Simple Object 序列化之后的 字符串 ， 这个 字符串 长度是 18 ， 前面的 “18” 就是指这个 。 以此类推 ， 第一个元素结束之后 ， 又是一个 “18” ， 这个 18 是指 第二个元素的长度 ， “18” 后面是空格 ， 空格 之后 就是 第二个元素 序列化之后的 字符串 。  

Value Type 数组 ， string 数组 的 原理 都包含在 上述 里了 ， 就不具体举例了 。

总之 ， ShareMemory.Serialization 可以支持 6 大类 数据类型 的 序列化 ： Value Type ， string ， Simple Object ， Walue Type 数组 ， string 数组 ， Simple Object 数组 。  

可以实际到 项目 里 运行 看一下 效果 就比较清楚了 。      ^ ^

ShareMemory.Serialization 可以作为一个 序列化库 单独使用 。  



ShareMemory 没有提供 对 数据操作 的 锁 机制（Lock） ， 因为 对 数据 的 锁机制 逻辑 比较 复杂 。 那么 ， 多个 客户端 线程 之间 怎么进行 通信协作 呢 ？ ShareMemory 提供了 与 数据无关 的 锁机制 。 Helper类 提供了 TryLock(lockName) 方法 和 UnLock(lockName, lockId) 方法 。 TryLock() 用来获取 锁 ， 参数 lockName 是 锁 的 名字 ， 参与协作 的 线程 间 可以 约定 一个 锁 的 名字 来 通信 。 TryLock() 方法 的 返回值 是 lockId ， 用来 标识 1 次 Lock ， 因为同一个 名字 的 锁 可能会多次 Lock 和 UnLock 。 UnLock 的时候需要 传入 lockId 参数 。 

这就是 ShareMemory 提供的 锁机制 ， 可以利用这个 锁机制 来 实现 多个 客户端 线程 间 的 通信协作 。 以此为基础 ， 开发者还可以实现各种丰富的线程间通信协作方式 。            












































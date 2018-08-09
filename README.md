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

Dic 提供 Set(key, value) 方法 ， Get<T>(key) 方法 ， TryGet<T>(key out value) 方法 ， Remove(key) 方法 。












































# ShareMemory
一个用 C# 实现的 No Sql 数据库 ， 也可以说是 分布式 缓存 ， 用于作为 集群 的 共享内存





ShareMemory 是 一个用 C# 实现的 No Sql 数据库 ， 也可以说是 分布式 缓存 ， 用于作为 集群 的 共享内存 。 

构建 集群 的 关键是 共享内存 。 ShareMemory 可以作为 集群 的 共享内存 ， 帮助 构建 集群 ， 这是 ShareMemory 的 第一 设计目标 。

ShareMemory 支持 2 种 数据结构 ： 字典（Dictionary）  队列（Queue） 。

支持 6 大类 数据类型 ： Value Type ， string ， Simple Object ， Walue Type 数组 ， string 数组 ， Simple Object 数组 。

可以将这 6 大类 数据类型 存放到 ShareMemory 。

Simple Object 是指 属性（Property）和 字段（Field） 类型 是 Value Type ， string 的 对象 ， 简单的说 ， 不支持 对象嵌套 。 这部分会在下面 序列化 的 部分详细介绍 。

ShareMemory 提供的 数据操作 都是 原子操作 ， 是 线程安全 的 。










During development I adhered to SOLID principles. That is why no service depends on implementation directly, the code is easy to cover by tests and is extendable.
I used repository pattern to abstract working with the storage. Used asynchronous methods signature not to break backward compatibility in case when mock in memory storage would be replaced by actual storage with asynchronous IO operations.
I avoided creating Employee class hierarchy because there were no benefits in it, only more complex code due to reflection.
When salary for entire company is calculated, I used cache to avoid redundand repeated calculations.
So far there is no way to update Employee information or delete it, because it wasn't in the task. But adding it wouldn't break salary calculation code.
Read and write locks are supported to avoid collisions between parallel operations.
Data consistensy ensured by delegation working with Employees instanses to EmployeeManagement assembly with internal constructor - only responsible services can create and manipulate data.
The the library could be used in an app that deployed in multiple Docker containers. IncreaseSettings are easily modifiable to use Env variable to change calculation settings.
If deployed in multiple containers, locks are redundand and can be removed - write consistensy supported by any DB engine.

using DucAnh2025.Services.SignalR;
using Microsoft.AspNetCore.SignalR;

public class DataService
{
    private readonly IHubContext<DataUpdateHub> _hubContext;

    public DataService(IHubContext<DataUpdateHub> hubContext)
    {
        _hubContext = hubContext;
    }

    // Giả sử bạn có một phương thức để thêm dữ liệu vào cơ sở dữ liệu
    public async Task AddDataAsync(string newData)
    {
        // Thực hiện thêm dữ liệu vào cơ sở dữ liệu
        // _dbContext.Add(newData);
        // await _dbContext.SaveChangesAsync();

        // Sau khi thêm dữ liệu thành công, thông báo cho tất cả client
        await _hubContext.Clients.All.SendAsync("ReceiveDataUpdate");
    }

    // Tương tự cho các phương thức sửa và xóa dữ liệu
    public async Task UpdateDataAsync(string updatedData)
    {
        // Cập nhật dữ liệu trong cơ sở dữ liệu
        // _dbContext.Update(updatedData);
        // await _dbContext.SaveChangesAsync();

        // Thông báo cho tất cả client
        await _hubContext.Clients.All.SendAsync("ReceiveDataUpdate");
    }

    public async Task DeleteDataAsync(string dataToDelete)
    {
        // Xóa dữ liệu từ cơ sở dữ liệu
        // _dbContext.Remove(dataToDelete);
        // await _dbContext.SaveChangesAsync();

        // Thông báo cho tất cả client
        await _hubContext.Clients.All.SendAsync("ReceiveDataUpdate");
    }
}

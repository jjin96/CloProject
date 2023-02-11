using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json.Serialization;
using CsvHelper;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System;
using System.Linq;

namespace Bl
{
    public class EmployeeService : IEmployeeService
    {
        private AppSetting? _appsettings;
        private IConfiguration _config;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IHostingEnvironment _env;

        public EmployeeService(IConfiguration config, ILogger<EmployeeService> logger, IHostingEnvironment env)
        {
            _config = config;
            _logger = logger;
            _env = env;
            var appsettings = new AppSetting();
            _config.GetSection("AppSettings").Bind(appsettings);
            _appsettings = appsettings;
        }

        public Middle<Employee> ReadAll(int page = 1, int pageSize = 10)
        {
            Middle<Employee> result = new Middle<Employee>();
            string path = string.Empty;

            List<Employee> mergeList = new List<Employee>();

            try
            {
                path = Path.Combine(_env.ContentRootPath, "Data", _appsettings?.JsonPath!);

                FileStream fs = File.OpenRead(path);

                mergeList = System.Text.Json.JsonSerializer.Deserialize<List<Employee>>(fs);                    
                fs.Close();

                path = Path.Combine(_env.ContentRootPath, "Data", _appsettings?.CsvPath!);

                using (StreamReader reader = new StreamReader(path))
                using (var csvReader = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    mergeList.AddRange(csvReader.GetRecords<Employee>().ToList());
                }
            

                result.List = mergeList.ToList();
                if(result.List.Count() >= (page * pageSize))
                {
                    result.List = result.List.Skip(page * pageSize).Take(pageSize);
                }
                else
                {
                    result.List = result.List.Take(result.List.Count());
                }

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}", ex);
            }

            return result;
        }

        public Middle<Employee> ReadName(string name)
        {
            Middle<Employee> result = ReadAll();

            string path = string.Empty;

            List<Employee> mergeList = new List<Employee>();

            try
            {
                path = Path.Combine(_env.ContentRootPath, "Data", _appsettings?.JsonPath!);

                FileStream fs = File.OpenRead(path);

                mergeList = System.Text.Json.JsonSerializer.Deserialize<List<Employee>>(fs);
                fs.Close();

                path = Path.Combine(_env.ContentRootPath, "Data", _appsettings?.CsvPath!);

                using (StreamReader reader = new StreamReader(path))
                using (var csvReader = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    mergeList.AddRange(csvReader.GetRecords<Employee>().ToList());
                }


                result.List = mergeList.ToList();
                result.List = result.List.Where(w => w.name.Equals(name));

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}", ex);
            }

            return result;
        }

        public Middle<Employee> Create(IFormFile file)
        {
            Middle<Employee> result = new Middle<Employee>();

            if(file == null)
            {
                result.Message = "등록할 사원정보가 없습니다.";
                return result;
            }

            try
            {
                // 확장자에 따라 csv, json 경로를 설정해줄 경로 변수
                string path = string.Empty;
                
                // 서버에 존재하는 파일을 읽어 저장할 변수
                string val = string.Empty;
               
                // 확장자 구하기
                string getExtension = Path.GetExtension(file.FileName);

                // json, csv를 Deserialize할 내용을 담을 리스트
                List<Employee> convertReq = new List<Employee>();

                if (getExtension.ToLower().Equals(".json"))
                {
                    // 중복체크를 위해 클라이언트에서 받은 json파일의 내용을 변환에 사용
                    StringBuilder reqGetStr = new StringBuilder();

                    // 클라이언트에서 받은 json파일의 내용을 string으로 저장하기 위해 사용
                    string reqVal = string.Empty;

                    path = Path.Combine(_env.ContentRootPath, "Data", _appsettings?.JsonPath!);

                    using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                    {
                        while (reader.Peek() >= 0)
                            reqGetStr.AppendLine(reader.ReadLine());
                    }
                    reqVal = reqGetStr.ToString();
                    
                    val = File.ReadAllText(path);

                    convertReq = JsonConvert.DeserializeObject<List<Employee>>(reqVal);

                    List<Employee> getEmployee = JsonConvert.DeserializeObject<List<Employee>>(val);

                    List<Employee> exceptEmployee = new List<Employee>();

                    if(getEmployee != null && getEmployee.Count > 0)
                    {
                        exceptEmployee = convertReq.Except(getEmployee).ToList();
                    }
                    else
                    {
                        exceptEmployee.AddRange(convertReq);
                    }

                    File.WriteAllText(path, JsonConvert.SerializeObject(exceptEmployee));
                    result.Message = "저장했습니다.";
                    result.IsSuccess = true;                                     
                }
                else
                {
                    path = path = Path.Combine(_env.ContentRootPath, "Data", _appsettings?.CsvPath!);

                    using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                    using (var csvReader = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                    {
                        convertReq = csvReader.GetRecords<Employee>().ToList();
                    }

                    List<Employee> getEmployee = new List<Employee>();
                    List <Employee> exceptEmployee = new List<Employee>();

                    using (StreamReader reader = new StreamReader(path))
                    using (CsvReader csvReader = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        getEmployee = csvReader.GetRecords<Employee>().ToList();                        

                        if(getEmployee != null && getEmployee.Count > 0)
                        {
                            exceptEmployee = convertReq.Except(getEmployee).ToList();
                        }
                        else
                        {
                            exceptEmployee.AddRange(convertReq);
                        }                      
                    }

                    using (var writer = new StreamWriter(path))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        for (int i = 0; i < exceptEmployee.Count; i++)
                        {
                            getEmployee.Add(exceptEmployee.ElementAt(i));
                        }

                        csv.WriteRecords(getEmployee);
                    }

                    result.Message = "저장했습니다.";
                    result.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                _logger.LogError($"{MethodBase.GetCurrentMethod().Name}", ex);
            }

            return result;
        }
    }
}
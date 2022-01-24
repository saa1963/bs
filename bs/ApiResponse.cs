using System.Text;

namespace bs
{
    public class ApiResponse
    {
        public int status { get; set; } //0 - все хорошо, 1,2,3... плохо по разным причинам
        public List<string> errors { get; set; } = new List<string>();
        public ApiResponse() { }
        public ApiResponse(int p_status, string error)
        {
            status = p_status;
            errors.Add(error);
        }
        public string AddMsg(string msg)
        {
            errors.Add(msg);
            return msg;
        }
        public override string ToString()
        {
            StringBuilder d = new StringBuilder();
            d.AppendJoin("\n", errors);
            return d.ToString();
        }
    }
}

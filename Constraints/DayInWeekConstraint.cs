namespace ArticleTask.Constraints;

public class DayInWeekConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if(!values.TryGetValue(routeKey, out var routeValue))
            return false;

        if (Enum.TryParse<DayOfWeek>(routeValue?.ToString(), out DayOfWeek day))
            return true;

        return false; 
    }
}
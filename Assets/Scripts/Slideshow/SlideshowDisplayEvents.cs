using UnityEngine.Events;

public static class SlideshowDisplayEvents
{
    public static UnityAction FirstSlide;
    public static void InvokeFirstSlide()
    {
        FirstSlide?.Invoke();
    }

    public static UnityAction LastSlide;
    public static void InvokeLastSlide()
    {
        LastSlide?.Invoke();
    }

    public static UnityAction MiddlingSlide;
    public static void InvokeMiddlingSlide()
    {
        MiddlingSlide?.Invoke();
    }

    public static UnityAction SingleSlide;
    public static void InvokeSingleSlide()
    {
        SingleSlide?.Invoke();
    }

    public static UnityAction ClearSlide;
    public static void InvokeClearSlide()
    {
        ClearSlide?.Invoke();
    }
}

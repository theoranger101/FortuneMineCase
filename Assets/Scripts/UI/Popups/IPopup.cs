namespace UI
{
    public interface IPopup
    {
        public void Initialize(object arg);
        public void OnPopupEvent(UIEvent evt);
        public void Toggle(bool visible);
    }
}
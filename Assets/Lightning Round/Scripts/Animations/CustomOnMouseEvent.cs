using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class CustomOnMouseEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler , IPointerDownHandler
{
    [SerializeField] private AudioClip _clipHover;
    [SerializeField] private AudioClip _clipClicked;
    [SerializeField] private bool _animate = true;
    private AudioSource _audioSource;

    private Button _btn;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _btn = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_clipHover != null)
            _audioSource.PlayOneShot(_clipHover);

        if (_animate)
            transform.LeanScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_animate)
            transform.LeanScale(new Vector3(1f, 1f, 1f), 0.2f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_clipClicked != null && _btn != null && _btn.interactable)
            _audioSource.PlayOneShot(_clipClicked);
    }


}

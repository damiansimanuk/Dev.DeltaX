const formData = reactive({
  name: "",
  email: "",
  password: "",
  age: "",
  url: "",
  comment: ""
});
const rules = {
  name: { required, minLength: minLength(2) },
  email: { required, email },
  password: { required, minLength: minLength(6) },
  age: { required, integer, maxLength: maxLength(3) },
  url: { url },
  comment: { required, minLength: minLength(10) }
};

const validate = useVuelidate(rules, toRefs(formData));
const save = () => {
  validate.value.$touch();
  console.log(validate.value);
  if (validate.value.$invalid) {
    Toastify({
      text: "Registration failed, please check the fileld form.",
      duration: 3000,
      newWindow: true,
      close: true,
      gravity: "bottom",
      position: "left",
      backgroundColor: "#D32929",
      stopOnFocus: true
    }).showToast();
  } else {
    Toastify({
      text: "Registration success!",
      duration: 3000,
      newWindow: true,
      close: true,
      gravity: "bottom",
      position: "left",
      backgroundColor: "#91C714",
      stopOnFocus: true
    }).showToast();
  }
};

return { validate, formData, save };

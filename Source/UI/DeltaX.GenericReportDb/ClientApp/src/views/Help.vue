<template>
  <div>
    <p>Hola {{ msg }}</p>
    <p>Hola {{ bind.text }}</p>
    <input v-model="bind.text" class="red" />
  </div>
</template>

<script>
export default {
  data: () => ({
    msg: "hola",
    bind: {
      text: "hola"
    }
  })
};
</script>

<style></style>
